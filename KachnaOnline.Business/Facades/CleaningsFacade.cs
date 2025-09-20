using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Cleanings;
using KachnaOnline.Business.Models.Cleanings;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.Cleanings;
using Microsoft.AspNetCore.Http;

namespace KachnaOnline.Business.Facades
{
    public class CleaningsFacade
    {
        private readonly ICleaningsService _cleaningsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CleaningsFacade(ICleaningsService cleaningsService, IHttpContextAccessor httpContextAccessor,
                               IMapper mapper, IUserService userService)
        {
            _cleaningsService = cleaningsService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userService = userService;
        }

        private async Task<CleaningDto> LinkUsernames(CleaningDto cleaningDto) 
        {
            var usersTask = _userService.GetUsers(cleaningDto.AssignedUsersIds.ToList());
            var users = await usersTask;

            cleaningDto.IdsToUsername = new Tuple<int, string>[users.Count()];

            var i = 0;
            foreach (var user in users)
            {
                cleaningDto.IdsToUsername[i] = Tuple.Create(user.Id, user.Nickname);
                i++;
            }

            return cleaningDto;
        }

        private async Task<IEnumerable<CleaningDto>> MapCleanings(ICollection<Cleaning> cleanings)
        {
            if (cleanings is not { Count: > 0 })
                return new List<CleaningDto>();

            var cleaningDtos = _mapper.Map<List<CleaningDto>>(cleanings);
            for (int cleaningI = 0; cleaningI < cleaningDtos.Count(); cleaningI++) {
                cleaningDtos[cleaningI] = await this.LinkUsernames(cleaningDtos[cleaningI]);
            }

            return cleaningDtos;
        }

        private async Task<CleaningDto> MapCleaning(Cleaning @cleaning)
        {
            var cleaningDto = _mapper.Map<CleaningDto>(@cleaning);

            cleaningDto = await this.LinkUsernames(cleaningDto);

            return cleaningDto;
        }

        /// <summary>
        /// Checks whether the current user is an cleanings manager.
        /// </summary>
        /// <returns>True if the current user is the cleanings manager, otherwise false.</returns>
        private bool IsUserCleaningsManager()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user != null && user.IsInRole(AuthConstants.CleaningsManager);
        }

        /// <summary>
        /// Get current user Id
        /// </summary>
        /// <exception cref="InvalidOperationException"> thrown when the current user could not be found.</exception>
        private int CurrentUserId =>
            int.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(IdentityConstants.IdClaim) ??
                      throw new InvalidOperationException("No valid user found in the current request."));


        /// <summary>
        /// Returns a list of current cleanings.
        /// </summary>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Cleanings.CleaningDto"/> of current cleanings or an empty list if no there is no cleaning currently.
        /// The list usually contains only one cleaning, more in case multiple next cleanings start at the same time.</returns>
        public async Task<IEnumerable<CleaningDto>> GetCurrentCleanings()
        {
            var cleanings = await _cleaningsService.GetCurrentCleanings();
            return await this.MapCleanings(cleanings);
        }

        /// <summary>
        /// Returns a list of cleanings that are held at the date and time specified by <paramref name="at"/>.
        /// </summary>
        /// <param name="at">Date and time to look for held cleanings.</param>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Cleanings.CleaningDto"/> of cleanings or an empty list if there is no cleaning at the specified date and time.</returns>
        public async Task<IEnumerable<CleaningDto>> GetCleanings(DateTime at)
        {
            var cleanings = await _cleaningsService.GetCleanings(at);
            return await this.MapCleanings(cleanings);
        }

        /// <summary>
        /// Returns a list of cleanings starting in the future, optionally only the ones starting in the timespan specified by <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">If not null, only returns cleanings with their from date being after or equal to the specified value.</param>
        /// <param name="to">If not null, only returns cleanings with their to date being before or equal to the specified value.</param>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Cleanings.CleaningDto"/> or an empty list if no there is no cleaning planned.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="from"/> or <paramref name="to"/> are invalid.</exception>
        public async Task<IEnumerable<CleaningDto>> GetCleanings(DateTime? from = null, DateTime? to = null)
        {
            var cleanings = await _cleaningsService.GetCleanings(from, to);
            return await this.MapCleanings(cleanings);
        }

        /// <summary>
        /// Returns a cleaning with the given ID.
        /// </summary>
        /// <param name="cleaningId">ID of the <see cref="KachnaOnline.Dto.Cleanings.CleaningDto"/> to return.</param>
        /// <param name="withLinkedStates">Whether to return linked states as well.</param>
        /// <returns>An <see cref="KachnaOnline.Dto.Cleanings.CleaningDto"/> with the given ID.</returns>
        /// <exception cref="CleaningNotFoundException"> thrown when a cleaning with the given
        /// <paramref name="cleaningId"/> does not exist.</exception>
        public async Task<CleaningDto> GetCleaning(int cleaningId)
        {
            return await this.MapCleaning(await _cleaningsService.GetCleaning(cleaningId));
        }

        /// <summary>
        /// Returns a list of next cleanings.
        /// </summary>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Cleanings.CleaningDto"/> planned as the next cleanings or an empty list if no cleaning is planned.
        /// The list usually contains only one cleaning, more in case multiple next cleanings start at the same time.</returns>
        public async Task<IEnumerable<CleaningDto>> GetNextPlannedCleanings()
        {
            var cleanings = await _cleaningsService.GetNextPlannedCleanings();
            return await this.MapCleanings(cleanings);
        }

        /// <summary>
        /// Plans a new cleaning.
        /// </summary>
        /// <param name="newCleaning">New cleaning data to create the new cleaning with.</param>
        /// <returns>Created cleaning with its ID attribute filled.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newCleaning"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newCleaning"/> has invalid attributes.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when the board game cannot be created.
        /// This can be caused by a database error.</exception>
        /// <exception cref="UserNotFoundException">Thrown when a user with the ID assigned to the cleaning does
        /// not exist.</exception>
        public async Task<ManagerCleaningDto> PlanCleaning(BaseCleaningDto newCleaning)
        {
            var newCleaningModel = _mapper.Map<NewCleaning>(newCleaning);
            newCleaningModel.MadeById = this.CurrentUserId;

            var createdCleaning = await _cleaningsService.PlanCleaning(newCleaningModel);

            return _mapper.Map<ManagerCleaningDto>(createdCleaning);
        }

        /// <summary>
        /// Modifies a cleaning with the given ID.
        /// </summary>
        /// <param name="cleaningId">ID of the cleaning to update.</param>
        /// <param name="baseCleaning"><see ref="KachnaOnline.Dto.Cleanings.BaseCleaningDto"/> representing the new state.</param>
        /// <exception cref="ArgumentNullException">Thrown when the passed <paramref name="modifiedCleaning"/> model is null.</exception>
        /// <exception cref="CleaningNotFoundException">Thrown when a cleaning with the given <paramref name="cleaningId"/>
        /// does not exist.</exception>
        /// <exception cref="CleaningReadOnlyException">Thrown when cleaning to be modified has already ended.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when cleaning modification has failed.</exception>
        public async Task ModifyCleaning(int cleaningId, BaseCleaningDto baseCleaning)
        {
            await _cleaningsService.ModifyCleaning(cleaningId, _mapper.Map<ModifiedCleaning>(baseCleaning));
        }

        /// <summary>
        /// Remove an cleaning with the given ID.
        /// </summary>
        /// <param name="cleaningId">ID of the cleaning to remove.</param>
        /// <exception cref="CleaningNotFoundException">Thrown when the cleaning with the given <paramref name="cleaningId"/> does not
        /// exist.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when the cleaning cannot be deleted.</exception>
        /// <exception cref="CleaningReadOnlyException">Thrown when cleaning to be removed has already ended.</exception>
        public async Task RemoveCleaning(int cleaningId)
        {
            await _cleaningsService.RemoveCleaning(cleaningId);
        }


        /// <summary>
        /// Join a cleaning with the given ID.
        /// </summary>
        /// <param name="cleaningId">ID of the cleaning to join.</param>
        /// <exception cref="CleaningNotFoundException">Thrown when the cleaning with the given <paramref name="cleaningId"/> does not
        /// exist.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when the cleaning cannot be joined.</exception>
        /// <exception cref="CleaningReadOnlyException">Thrown when cleaning to be joined has already ended.</exception>
        public async Task JoinCleaning(int cleaningId)
        {
            var joinedCleaningModel = _mapper.Map<ModifiedCleaning>(await this.GetCleaning(cleaningId));
            if (!joinedCleaningModel.AssignedUsersIds.Contains(this.CurrentUserId))
            {
                joinedCleaningModel.AssignedUsersIds.Add(this.CurrentUserId);
                await _cleaningsService.ModifyCleaning(cleaningId, joinedCleaningModel);
            }
        }
        
        /// <summary>
        /// Leaves a cleaning with the given ID.
        /// </summary>
        /// <param name="cleaningId">ID of the cleaning to leave.</param>
        /// <exception cref="CleaningNotFoundException">Thrown when the cleaning with the given <paramref name="cleaningId"/> does not
        /// exist.</exception>
        /// <exception cref="CleaningManipulationFailedException">Thrown when the cleaning cannot be left.</exception>
        /// <exception cref="CleaningReadOnlyException">Thrown when cleaning to be left has already ended.</exception>
        public async Task LeaveCleaning(int cleaningId)
        {
            var joinedCleaningModel = _mapper.Map<ModifiedCleaning>(await this.GetCleaning(cleaningId));
            if (joinedCleaningModel.AssignedUsersIds.Contains(this.CurrentUserId))
            {
                joinedCleaningModel.AssignedUsersIds.Remove(this.CurrentUserId);
                await _cleaningsService.ModifyCleaning(cleaningId, joinedCleaningModel);
            }
        }
    }
}
