// RepeatingStatesFacade.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.ClubStates;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.Business.Facades
{
    public class RepeatingStatesFacade
    {
        private readonly IClubStateService _clubStateService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<RepeatingStatesFacade> _logger;

        public RepeatingStatesFacade(IClubStateService clubStateService, IHttpContextAccessor httpContextAccessor,
            IUserService userService, IMapper mapper, ILogger<RepeatingStatesFacade> logger)
        {
            _clubStateService = clubStateService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        private int CurrentUserId =>
            int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(IdentityConstants.IdClaim) ??
                      throw new InvalidOperationException("No valid user found in the current request."));

        private bool IsUserManager
            => _httpContextAccessor.HttpContext?.User?.IsInRole(AuthConstants.StatesManager) ?? false;

        private async Task<StateMadeByDto> MakeMadeByDto(int? userId)
        {
            return await _userService.GetUserMadeByDto(userId, this.IsUserManager);
        }

        private async Task<List<StateDto>> MapStateCollection(IEnumerable<State> states)
        {
            var dtos = new List<StateDto>();
            foreach (var state in states)
            {
                StateDto dto;
                if (state.Ended.HasValue)
                {
                    var pastStateDto = _mapper.Map<PastStateDto>(state);
                    if (state.ClosedById.HasValue)
                    {
                        pastStateDto.ClosedBy = await this.MakeMadeByDto(state.ClosedById);
                    }

                    dto = pastStateDto;
                }
                else
                {
                    dto = _mapper.Map<StateDto>(state);
                }

                dto.MadeBy = await this.MakeMadeByDto(state.MadeById);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<List<RepeatingStateDto>> Get(DateTime effectiveAt)
        {
            var states = await _clubStateService.GetRepeatingStates(effectiveAt);

            if (!this.IsUserManager)
                return _mapper.Map<List<RepeatingStateDto>>(states);

            var dtos = new List<RepeatingStateDto>();
            foreach (var state in states)
            {
                var dto = _mapper.Map<RepeatingStateManagerDto>(state);
                dto.MadeBy = await this.MakeMadeByDto(state.MadeById);
                dtos.Add(dto);
            }

            return dtos;
        }

        /// <summary>
        /// Get all repeating states, optionally bound their <see cref="RepeatingStateDto.EffectiveFrom"/> with
        /// <paramref name="effectiveFrom"/> and <paramref name="effectiveTo"/>.
        /// </summary>
        /// <remarks>
        /// Only used by status managers.
        /// </remarks>
        public async Task<List<RepeatingStateDto>> Get(DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            var states = await _clubStateService.GetRepeatingStates(effectiveFrom, effectiveTo);

            var dtos = new List<RepeatingStateDto>();
            foreach (var state in states)
            {
                var dto = _mapper.Map<RepeatingStateManagerDto>(state);
                dto.MadeBy = await this.MakeMadeByDto(state.MadeById);
                dtos.Add(dto);
            }

            return dtos;
        }

        /// <summary>
        /// Get all planned states that were based on the given repeating state.
        /// </summary>
        /// <remarks>
        /// Only used by status managers.
        /// </remarks>
        public async Task<List<StateDto>> GetLinkedStates(int repeatingStateId, bool futureOnly)
        {
            var states = await _clubStateService.GetStatesForRepeatingState(repeatingStateId, futureOnly);
            if (states.Count == 0)
                return null;

            var dtos = await this.MapStateCollection(states);
            return dtos;
        }

        public async Task<RepeatingStatePlanningResultDto> Plan(RepeatingStatePlanningDto data)
        {
            var newState = _mapper.Map<RepeatingState>(data);
            newState.MadeById = this.CurrentUserId;
            newState.Id = default;

            var result = await _clubStateService.MakeRepeatingState(newState);
            var dto = new RepeatingStatePlanningResultDto();

            if (result.HasOverlappingStates)
            {
                dto.CollidingStates = await this.MapStateCollection(result.OverlappingStates);
            }

            newState = await _clubStateService.GetRepeatingState(result.TargetRepeatingStateId);

            if (newState is null)
            {
                _logger.LogError("Newly created repeating state not found.");
            }
            else
            {
                dto.TargetRepeatingState = _mapper.Map<RepeatingStateManagerDto>(newState);
                dto.TargetRepeatingState.MadeBy = await this.MakeMadeByDto(newState.MadeById);
            }

            return dto;
        }

        public async Task<RepeatingStatePlanningResultDto> Modify(int id, RepeatingStateModificationDto data)
        {
            var modification = _mapper.Map<RepeatingStateModification>(data);
            modification.RepeatingStateId = id;
            var result = await _clubStateService.ModifyRepeatingState(modification, this.CurrentUserId);
            var dto = new RepeatingStatePlanningResultDto();

            if (result.HasOverlappingStates)
            {
                dto.CollidingStates = await this.MapStateCollection(result.OverlappingStates);
            }

            var modifiedState = await _clubStateService.GetRepeatingState(result.TargetRepeatingStateId);
            if (modifiedState is null)
            {
                _logger.LogError("Modified repeating state not found.");
            }
            else
            {
                dto.TargetRepeatingState = _mapper.Map<RepeatingStateManagerDto>(modifiedState);
                dto.TargetRepeatingState.MadeBy = await this.MakeMadeByDto(modifiedState.MadeById);
            }

            return dto;
        }

        public async Task<RepeatingStateManagerDto> Delete(int id)
        {
            await _clubStateService.RemoveRepeatingState(id, this.CurrentUserId);
            var state = await _clubStateService.GetRepeatingState(id);
            var stateDto = _mapper.Map<RepeatingStateManagerDto>(state);
            if (stateDto != null)
            {
                stateDto.MadeBy = await this.MakeMadeByDto(state.MadeById);
            }

            return stateDto;
        }
    }
}
