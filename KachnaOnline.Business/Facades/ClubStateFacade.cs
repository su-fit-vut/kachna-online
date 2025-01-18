using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.ClubStates;
using KachnaOnline.Dto.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StateType = KachnaOnline.Dto.ClubStates.StateType;

namespace KachnaOnline.Business.Facades
{
    public class ClubStateFacade
    {
        private static readonly DateTime KachnaForEveryoneStart = new DateTime(2025, 1, 1);

        private readonly IClubStateService _clubStateService;
        private readonly IUserService _userService;
        private readonly IKisService _kisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ClubStateOptions> _optionsMonitor;
        private readonly ILogger<ClubStateFacade> _logger;

        public ClubStateFacade(IClubStateService clubStateService, IUserService userService,
            IKisService kisService, IHttpContextAccessor httpContextAccessor, IMapper mapper,
            IOptionsMonitor<ClubStateOptions> optionsMonitor, ILogger<ClubStateFacade> logger)
        {
            _clubStateService = clubStateService;
            _userService = userService;
            _kisService = kisService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _logger = logger;
        }

        private int CurrentUserId =>
            int.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(IdentityConstants.IdClaim) ??
                      throw new InvalidOperationException("No valid user found in the current request."));

        private bool IsUserManager
            => _httpContextAccessor.HttpContext?.User?.IsInRole(AuthConstants.StatesManager) ?? false;

        private async Task<MadeByUserDto> MakeMadeByDto(int? userId)
        {
            return await _userService.GetUserMadeByDto(userId, this.IsUserManager);
        }

        private async Task<StateDto> MapState(State state)
        {
            if (state is null)
                return null;

            if (!this.IsUserManager)
            {
                if (!string.IsNullOrEmpty(state.NoteInternal))
                {
                    state.NoteInternal = null;
                }

                if (!string.IsNullOrEmpty(state.FollowingState?.NoteInternal))
                {
                    state.FollowingState.NoteInternal = null;
                }

                if (state.Type == Models.ClubStates.StateType.Private)
                {
                    state.MadeById = null;
                    state.ClosedById = null;
                    if (state.Start < KachnaForEveryoneStart)
                        state.NotePublic = null;
                }
            }

            StateDto dto;
            if (state.Ended.HasValue)
            {
                var pastStateDto = _mapper.Map<PastStateDto>(state);
                if (state.ClosedById.HasValue)
                {
                    pastStateDto.ClosedByUser = await this.MakeMadeByDto(state.ClosedById);
                }

                dto = pastStateDto;
            }
            else
            {
                dto = _mapper.Map<StateDto>(state);
            }

            dto.MadeByUser = await this.MakeMadeByDto(state.MadeById);
            if (state.FollowingState?.MadeById != null)
            {
                ((StateDto)dto.FollowingState).MadeByUser = await this.MakeMadeByDto(state.FollowingState.MadeById);
            }

            return dto;
        }

        private async Task<StatePlanningResultDto> MakeStatePlanningResultDto(StatePlanningResult result)
        {
            var dto = new StatePlanningResultDto();

            if (result.HasOverlappingStates)
            {
                dto.ConflictResultDto = new StatePlanningConflictResultDto { CollidingStates = new List<StateDto>() };
                foreach (var overlappingStateId in result.OverlappingStatesIds)
                {
                    var state = await _clubStateService.GetState(overlappingStateId);
                    if (state != null)
                    {
                        dto.ConflictResultDto.CollidingStates.Add(await this.MapState(state));
                    }
                }

                return dto;
            }

            var targetState = await _clubStateService.GetState(result.TargetStateId);
            if (targetState is null)
            {
                _logger.LogCritical("Data inconsistency: created/modified state not found.");
                throw new StatePlanningException("Data inconsistency: created/modified state not found.");
            }

            dto.SuccessResultDto = new StatePlanningSuccessResultDto
            {
                NewState = await this.MapState(targetState)
            };

            if (result.ModifiedPreviousStateId.HasValue)
            {
                var modifiedState = await _clubStateService.GetState(result.ModifiedPreviousStateId.Value);
                if (modifiedState.FollowingState?.Id == targetState.Id)
                {
                    // Don't duplicate the data
                    modifiedState.FollowingState = null;
                }

                dto.SuccessResultDto.ModifiedState = await this.MapState(modifiedState);
            }

            return dto;
        }

        public async Task<StateDto> GetCurrent()
        {
            var currentState = await _clubStateService.GetCurrentState();
            var dto = await this.MapState(currentState);
            return dto;
        }

        public async Task<StateDto> Get(int id)
        {
            var state = await _clubStateService.GetState(id);
            if (state is null)
                return null;

            var dto = await this.MapState(state);
            return dto;
        }

        public async Task<List<StateDto>> GetNearOrBetween(DateTime? from, DateTime? to)
        {
            DateTime actualFrom, actualTo;
            var currentMax = _optionsMonitor.CurrentValue.MaximumDaysSpanForStatesListAllowed;

            if (!from.HasValue && !to.HasValue)
            {
                actualFrom = DateTime.Now.Date.AddDays(-DateTime.Now.Day);
                actualTo = actualFrom.AddDays(currentMax);
            }
            else if (from.HasValue && !to.HasValue)
            {
                actualFrom = from.Value;
                actualTo = actualFrom.AddDays(currentMax);
            }
            else if (!from.HasValue)
            {
                actualTo = to.Value;
                actualFrom = actualTo.AddDays(-currentMax);
            }
            else
            {
                var difference = to.Value - from.Value;
                if (difference < TimeSpan.Zero || difference > TimeSpan.FromDays(currentMax))
                    return null;

                actualFrom = from.Value;
                actualTo = to.Value;
            }

            var states = await _clubStateService.GetStates(actualFrom, actualTo);
            var result = new List<StateDto>();
            var isManager = this.IsUserManager;

            foreach (var state in states)
            {
                var dto = await this.MapState(state);
                result.Add(dto);
            }

            return result;
        }

        public async Task<StateDto> GetNext(StateType? type)
        {
            var state = await _clubStateService.GetNextPlannedState(
                _mapper.Map<Models.ClubStates.StateType?>(type));

            var dto = await this.MapState(state);
            return dto;
        }

        public async Task<StatePlanningResultDto> PlanNew(StatePlanningDto newStateDto)
        {
            if (newStateDto.Start.HasValue)
            {
                newStateDto.Start = newStateDto.Start.Value.RoundToMinutes();
                if (newStateDto.Start < DateTime.Now.RoundToMinutes())
                    throw new ArgumentException("Cannot plan a state in the past.");

                if (newStateDto.PlannedEnd.HasValue &&
                    newStateDto.PlannedEnd.Value.RoundToMinutes() <= newStateDto.Start)
                    throw new ArgumentException("The state's planned end must come after its start.");
            }

            var newState = _mapper.Map<NewState>(newStateDto);
            newState.MadeById = this.CurrentUserId;

            StatePlanningResult result;
            try
            {
                result = await _clubStateService.PlanState(newState);
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("The state's planned end must come after its start.");
            }

            var dto = await this.MakeStatePlanningResultDto(result);
            return dto;
        }

        public async Task<StatePlanningResultDto> ModifyCurrent(StateModificationDto data)
        {
            var modification = _mapper.Map<StateModification>(data);
            var result = await _clubStateService.ModifyState(modification, this.CurrentUserId);
            var dto = await this.MakeStatePlanningResultDto(result);

            return dto;
        }

        public async Task<StatePlanningResultDto> Modify(int id, StateModificationDto data)
        {
            var modification = _mapper.Map<StateModification>(data);
            modification.StateId = id;

            var result = await _clubStateService.ModifyState(modification, this.CurrentUserId);
            var dto = await this.MakeStatePlanningResultDto(result);

            return dto;
        }

        public async Task Delete(int id)
        {
            await _clubStateService.RemovePlannedState(id);
        }

        public async Task CloseCurrent()
        {
            await _clubStateService.CloseNow(this.CurrentUserId);
        }

        /// <summary>
        /// Unlinks the specified linked state from any event.
        /// </summary>
        /// <param name="stateId">ID of the planned state to be unlinked from any event.</param>
        /// <exception cref="StateReadOnlyException">Thrown when planned state to be unlinked from the event has already started or ended.</exception>
        /// <exception cref="StateNotFoundException">Thrown when planned state to be unlinked from the event has not been found.</exception>
        /// <exception cref="StateNotAssociatedToEventException">Thrown when planned states is not associated to any event.</exception>
        public async Task UnlinkStateFromEvent(int stateId)
        {
            await _clubStateService.UnlinkStateFromEvent(stateId);
        }

        public async Task<LegacyStateDto> GetCurrentLegacy()
        {
            var currentState = await _clubStateService.GetCurrentState();
            var nextState = await _clubStateService.GetNextPlannedState(null, false);
            var nextBarState = await _clubStateService.GetNextPlannedState(Models.ClubStates.StateType.OpenBar);
            var tapInfo = await _kisService.GetTapInfo();

            string[] beers = null;
            if (tapInfo is { Count: > 0 })
            {
                beers = tapInfo.Select(t =>
                {
                    var article = t.OfferedArticles?.FirstOrDefault();
                    if (article is null)
                        return null;

                    return $"{article.Name} ({article.Price} Kč)";
                }).Where(t => t != null).ToArray();

                if (beers.Length == 0)
                {
                    beers = null;
                }
            }

            var dto = new LegacyStateDto
            {
                State = _mapper.Map<StateType>(currentState.Type),
                LastChange = currentState.Start,
                ExpectedEnd = currentState.PlannedEnd,
                NextPlannedState = _mapper.Map<StateType?>(nextState?.Type) ?? StateType.Closed,
                NextStateDateTime = nextState?.Start,
                NextOpeningDateTime = nextBarState?.Start,
                Note = currentState.NotePublic,
                BeersOnTap = beers
            };

            return dto;
        }
    }
}
