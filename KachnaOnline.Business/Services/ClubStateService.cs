using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.StatePlanning.Abstractions;
using KachnaOnline.Business.Utils;
using KachnaOnline.Data.Entities.ClubStates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepeatingStateEntity = KachnaOnline.Data.Entities.ClubStates.RepeatingState;
using RepeatingState = KachnaOnline.Business.Models.ClubStates.RepeatingState;
using StateType = KachnaOnline.Business.Models.ClubStates.StateType;

namespace KachnaOnline.Business.Services
{
    public class ClubStateService : IClubStateService
    {
        /// <summary>
        /// Represents information whether a certain state is the current state, a past state or a planned (future) one.
        /// Used by <see cref="ClubStateService.ModifyState"/>.
        /// </summary>
        private enum StateRelativeTime
        {
            Past,
            Current,
            Planned
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStatePlannerService _statePlannerService;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptionsMonitor<ClubStateOptions> _optionsMonitor;
        private readonly ILogger<ClubStateService> _logger;
        private readonly IPlannedStatesRepository _stateRepository;
        private readonly IRepeatingStatesRepository _repeatingStatesRepository;

        private const int StatePlanningEnterTimeout = 1000;
        private static readonly SemaphoreSlim StatePlanningSemaphore = new(1);

        /// <summary>
        /// Attempts to acquire the state planning lock.
        /// Throws if the lock isn't acquired in <see cref="StatePlanningEnterTimeout"/> milliseconds.
        /// </summary>
        /// <exception cref="StatePlanningException">Thrown if the lock isn't acquired in <see cref="StatePlanningEnterTimeout"/> milliseconds.</exception>
        private static async Task EnsureLock()
        {
            var hasLock = await StatePlanningSemaphore.WaitAsync(StatePlanningEnterTimeout);
            if (!hasLock)
                throw new StatePlanningException("Cannot acquire the state planning lock.");
        }

        public ClubStateService(IUnitOfWork unitOfWork, IMapper mapper, IStatePlannerService statePlannerService,
            IUserService userService, IServiceProvider serviceProvider,
            IOptionsMonitor<ClubStateOptions> optionsMonitor, ILogger<ClubStateService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _statePlannerService = statePlannerService;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _optionsMonitor = optionsMonitor;
            _logger = logger;

            _stateRepository = unitOfWork.PlannedStates;
            _repeatingStatesRepository = unitOfWork.RepeatingStates;
        }

        /// <summary>
        /// Attempts to retrieve a user with the specified ID and throws if no such user exists.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <exception cref="UserNotFoundException">Thrown if no user with such ID exists.</exception>
        private async Task EnsureUser(int userId)
        {
            var user = await _userService.GetUser(userId);
            if (user is null)
                throw new UserNotFoundException(userId);
        }

        private async Task<State> GetClosedAfter(PlannedState previousStateEntity)
        {
            var nextStateEntity = await _stateRepository.GetNearest();

            // It is currently closed.
            var closedStateModel = new State()
            {
                Start = previousStateEntity?.Ended ?? DateTime.MinValue,
                PlannedEnd = nextStateEntity?.Start,
                Type = StateType.Closed,
                MadeById = previousStateEntity?.ClosedById
            };

            if (nextStateEntity != null)
                closedStateModel.FollowingState = _mapper.Map<State>(nextStateEntity);

            return closedStateModel;
        }


        /// <inheritdoc />
        public async Task<State> GetCurrentState()
        {
            var stateEntity = await _stateRepository.GetCurrent(includeNext: true);
            if (stateEntity is null)
            {
                var previousStateEntity = await _stateRepository.GetLastEnded();
                var closedStateModel = await this.GetClosedAfter(previousStateEntity);
                return closedStateModel;
            }

            var stateModel = _mapper.Map<State>(stateEntity);
            return stateModel;
        }

        /// <inheritdoc />
        public async Task<State> GetState(int id)
        {
            var stateEntity = await _stateRepository.GetWithNext(id);
            if (stateEntity is null)
                return null;

            var stateModel = _mapper.Map<State>(stateEntity);
            return stateModel;
        }

        /// <inheritdoc />
        public async Task<State> GetNextPlannedState(StateType? type, bool enablePrivate)
        {
            if (type == StateType.Closed)
            {
                var currentState = await this.GetCurrentState();
                if (currentState.Type == StateType.Closed)
                    return currentState;

                while (currentState.FollowingState != null)
                {
                    currentState = await this.GetState(currentState.FollowingState.Id);
                }
            }

            if (type == StateType.Private && !enablePrivate)
                return null;


            var typeMapped = _mapper.Map<KachnaOnline.Data.Entities.ClubStates.StateType?>(type);
            var stateEntity = await _stateRepository.GetNearest(typeMapped, null, true);
            if (stateEntity is null)
                return null;

            while (stateEntity.State == KachnaOnline.Data.Entities.ClubStates.StateType.Private && !enablePrivate)
            {
                stateEntity = await _stateRepository.GetNearest(typeMapped, stateEntity.Ended ?? stateEntity.PlannedEnd,
                    true);
                if (stateEntity is null)
                    return null;
            }

            var stateModel = _mapper.Map<State>(stateEntity);
            return stateModel;
        }

        /// <inheritdoc />
        public async Task<ICollection<State>> GetStates(DateTime from, DateTime to)
        {
            if (to < from)
            {
                throw new ArgumentException(
                    $"The {nameof(to)} argument must not be a datetime before {nameof(@from)}.",
                    nameof(to));
            }

            var currentMax = _optionsMonitor.CurrentValue.MaximumDaysSpanForStatesListAllowed;
            if (to - from > TimeSpan.FromDays(currentMax))
                throw new ArgumentException($"The maximum time span to get states for is {currentMax} days.");

            var stateEntities =
                _stateRepository.GetStartingBetween(from.RoundToMinutes(), to.RoundToMinutes(), false);
            var returnList = new List<State>();
            await foreach (var stateEntity in stateEntities)
            {
                returnList.Add(_mapper.Map<State>(stateEntity));
            }

            return returnList;
        }

        /// <inheritdoc />
        public async Task<ICollection<RepeatingState>> GetRepeatingStates(DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null)
        {
            var repeatingStateEntities = _repeatingStatesRepository.All(effectiveFrom, effectiveTo);
            var returnList = new List<RepeatingState>();
            await foreach (var stateEntity in repeatingStateEntities)
            {
                returnList.Add(_mapper.Map<RepeatingState>(stateEntity));
            }

            return returnList;
        }

        /// <inheritdoc />
        public async Task<ICollection<RepeatingState>> GetRepeatingStates(DateTime effectiveAt)
        {
            var repeatingStateEntities = _repeatingStatesRepository.EffectiveAt(effectiveAt);
            var returnList = new List<RepeatingState>();
            await foreach (var stateEntity in repeatingStateEntities)
            {
                returnList.Add(_mapper.Map<RepeatingState>(stateEntity));
            }

            return returnList;
        }

        public async Task<RepeatingState> GetRepeatingState(int id)
        {
            var repeatingStateEntity = await _repeatingStatesRepository.Get(id);
            return _mapper.Map<RepeatingState>(repeatingStateEntity);
        }

        /// <inheritdoc />
        public async Task<ICollection<State>> GetStatesForRepeatingState(int repeatingStateId,
            bool futureOnly = true)
        {
            var stateEntities = _stateRepository.GetForRepeatingState(repeatingStateId,
                futureOnly ? DateTime.Now : null, false);

            var returnList = new List<State>();
            await foreach (var stateEntity in stateEntities)
            {
                returnList.Add(_mapper.Map<State>(stateEntity));
            }

            return returnList;
        }

        /// <inheritdoc />
        public async Task<RepeatingStatePlanningResult> MakeRepeatingState(RepeatingState newRepeatingState)
        {
            var dateFrom = newRepeatingState.EffectiveFrom.Date;
            var dateTo = newRepeatingState.EffectiveTo.Date;

            if (dateTo <= dateFrom)
            {
                throw new ArgumentException(
                    $"The {nameof(RepeatingState.EffectiveTo)} date must come after the {nameof(RepeatingState.EffectiveFrom)}.");
            }

            var timeFrom = newRepeatingState.TimeFrom.RoundToMinutes();
            var timeTo = newRepeatingState.TimeTo.RoundToMinutes();
            if (timeTo <= timeFrom)
            {
                throw new ArgumentException(
                    $"The {nameof(RepeatingState.TimeTo)} time must come after the {nameof(RepeatingState.TimeFrom)}.");
            }

            if (!Enum.IsDefined(typeof(DayOfWeek), newRepeatingState.DayOfWeek))
                throw new ArgumentException($"The value of {nameof(RepeatingState.DayOfWeek)} is not valid.");

            if (!Enum.IsDefined(typeof(StateType), newRepeatingState.State))
                throw new ArgumentException($"The value of {nameof(RepeatingState.State)} is not valid.");

            if (newRepeatingState.State == StateType.Closed)
                throw new ArgumentException("Cannot plan a 'Closed' repeating state.");

            // Ensure the user exists
            await this.EnsureUser(newRepeatingState.MadeById);

            var newEntity = _mapper.Map<RepeatingStateEntity>(newRepeatingState);

            // Get the first actual day of occurrence
            var daysToAdd = ((int)newRepeatingState.DayOfWeek - (int)dateFrom.DayOfWeek + 7) % 7;
            dateFrom = dateFrom.AddDays(daysToAdd);

            // Store the rounded values instead of the original ones
            newEntity.EffectiveFrom = dateFrom;
            newEntity.EffectiveTo = dateTo;
            newEntity.TimeFrom = timeFrom;
            newEntity.TimeTo = timeTo;

            try
            {
                await _repeatingStatesRepository.Add(newEntity);
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot save a new RepeatingState.");
                throw new StatePlanningException("Cannot save the new repeating state.", e);
            }


            await EnsureLock();
            try
            {
                return await this.PlanStatesForRepeatingState(newEntity, dateFrom);
            }
            catch (Exception e)
            {
                try
                {
                    await _repeatingStatesRepository.Delete(newEntity);
                    await _unitOfWork.SaveChanges();
                    throw new StatePlanningException("Cannot plan a new repeating state.", e);
                }
                catch (Exception deleteE)
                {
                    _logger.LogError(deleteE,
                        "Cannot remove a repeating state that should be removed because of an exception that has occurred when planning states.");
                    throw new AggregateException(deleteE, e);
                }
            }
            finally
            {
                StatePlanningSemaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task RemoveRepeatingState(int repeatingStateId, int removedById)
        {
            var stateEntity = await _repeatingStatesRepository.Get(repeatingStateId);
            if (stateEntity is null)
                throw new RepeatingStateNotFoundException(repeatingStateId);

            // The repeating state has ended already – don't do anything
            if (stateEntity.EffectiveTo <= DateTime.Today)
                throw new RepeatingStateReadOnlyException(repeatingStateId);

            // The repeating state hasn't started yet – delete the entity, the linked states will be cascaded
            if (stateEntity.EffectiveFrom > DateTime.Today)
            {
                _logger.LogInformation(
                    "User {UserId} is removing repeating state {RepeatingStateId} that hasn't started yet.",
                    removedById, repeatingStateId);

                await EnsureLock();
                try
                {
                    await _repeatingStatesRepository.Delete(stateEntity);
                    await _unitOfWork.SaveChanges();
                    await _statePlannerService.NotifyPlanChanged();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Cannot remove repeating state {Id}.", repeatingStateId);
                    throw new StatePlanningException("Cannot remove the specified repeating state.", e,
                        repeatingStateId);
                }
                finally
                {
                    StatePlanningSemaphore.Release();
                }

                return;
            }

            // The repeating state has started – delete all linked states in the future
            // This is the same behaviour as changing the EffectiveTo date to today
            var modification = new RepeatingStateModification()
            {
                EffectiveTo = DateTime.Today,
                RepeatingStateId = repeatingStateId
            };

            await this.ModifyRepeatingState(modification, removedById);
        }

        /// <inheritdoc />
        public async Task<RepeatingStatePlanningResult> ModifyRepeatingState(
            RepeatingStateModification modification,
            int changeMadeByUserId)
        {
            var stateEntity = await _repeatingStatesRepository.Get(modification.RepeatingStateId);
            if (stateEntity is null)
                throw new RepeatingStateNotFoundException(modification.RepeatingStateId);

            if (modification.EffectiveTo.HasValue)
                modification.EffectiveTo = modification.EffectiveTo.Value.Date;

            if (stateEntity.EffectiveTo <= DateTime.Today)
                throw new RepeatingStateReadOnlyException(modification.RepeatingStateId);

            if (modification.State is StateType.Closed)
                throw new ArgumentException("Cannot plan a 'Closed' repeating state.");

            if (modification.EffectiveTo < DateTime.Today)
            {
                throw new ArgumentException(
                    $"Cannot change a repeating state's {nameof(RepeatingState.EffectiveTo)} to a date before today.");
            }


            // Check user and modify MadeById for the R.S. and the linked states
            await CheckAndModifyUserReferences();

            // Apply note modifications
            ModifyNotes();

            if (modification.State.HasValue)
            {
                stateEntity.State =
                    _mapper.Map<KachnaOnline.Data.Entities.ClubStates.StateType>(modification.State.Value);
            }

            var hasStateAtEnd = false;
            // Change the properties of the linked states *in the future*
            await foreach (var state in _stateRepository.GetForRepeatingState(stateEntity.Id, DateTime.Now))
            {
                if (state.Start.Date == stateEntity.EffectiveTo)
                {
                    hasStateAtEnd = true;
                }

                state.NoteInternal = stateEntity.NoteInternal;
                state.NotePublic = stateEntity.NotePublic;
                state.State = stateEntity.State;
            }

            // Apply EffectiveTo modifications
            if (modification.EffectiveTo.HasValue && modification.EffectiveTo.Value != stateEntity.EffectiveTo)
            {
                await EnsureLock();
                try
                {
                    if (modification.EffectiveTo < stateEntity.EffectiveTo)
                    {
                        // Shortening the repeating state – delete the states and that's it

                        var deleteStatesFromDate = modification.EffectiveTo.Value;
                        if (modification.EffectiveTo.Value.DayOfWeek == stateEntity.DayOfWeek)
                        {
                            // Make sure the final day stays included if it a day of the state's occurrence
                            deleteStatesFromDate = deleteStatesFromDate.AddDays(1);
                        }

                        await foreach (var state in _stateRepository.GetForRepeatingState(stateEntity.Id,
                                           deleteStatesFromDate))
                        {
                            await _stateRepository.Delete(state);
                        }

                        stateEntity.EffectiveTo = modification.EffectiveTo.Value;
                        await _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        // Prolonging the repeating state – plan new states
                        var originalEffectiveTo = stateEntity.EffectiveTo;
                        var daysToAdd = ((int)stateEntity.DayOfWeek - (int)originalEffectiveTo.DayOfWeek + 7) % 7;
                        if (hasStateAtEnd)
                        {
                            // Ensure that if the repeating state is planned so that its EffectiveTo is equal to the
                            // final state's start date, this final state won't be replanned
                            daysToAdd += 7;
                        }

                        var makeStatesFrom = originalEffectiveTo.AddDays(daysToAdd);

                        stateEntity.EffectiveTo = modification.EffectiveTo.Value;
                        await _unitOfWork.SaveChanges();
                        return await this.PlanStatesForRepeatingState(stateEntity, makeStatesFrom);
                    }
                }
                finally
                {
                    StatePlanningSemaphore.Release();
                }
            }
            else
            {
                // We still have to save the changed properties somewhere
                await _unitOfWork.SaveChanges();
            }

            return new RepeatingStatePlanningResult()
            {
                TargetRepeatingStateId = modification.RepeatingStateId
            };

            async Task CheckAndModifyUserReferences()
            {
                var changeMadeByUserRoles = await _userService.GetUserRoles(changeMadeByUserId);
                if (changeMadeByUserRoles is null)
                    throw new UserNotFoundException(changeMadeByUserId);

                if (modification.MadeById.HasValue)
                {
                    if (!changeMadeByUserRoles.Any(r => r == AuthConstants.Admin))
                    {
                        throw new UserUnprivilegedException(changeMadeByUserId, AuthConstants.Admin,
                            $"change the {nameof(RepeatingState.MadeById)} attribute of states");
                    }

                    var newMadeByUser = await _userService.GetUser(modification.MadeById.Value);
                    if (newMadeByUser is null)
                        throw new UserNotFoundException(modification.MadeById.Value);

                    _logger.LogInformation(
                        "User {UserId} changed MadeById of Repeating State {RepeatingStateId} from {OriginalMadeById} to {NewMadeById}.",
                        changeMadeByUserId, stateEntity.MadeById, stateEntity.MadeById,
                        modification.MadeById.Value);

                    stateEntity.MadeById = modification.MadeById.Value;

                    await foreach (var state in _stateRepository.GetForRepeatingState(stateEntity.Id))
                    {
                        state.MadeById = modification.MadeById.Value;
                    }
                }
            }

            void ModifyNotes()
            {
                var modified = false;
                if (modification.NotePublic != null && modification.NotePublic != stateEntity.NotePublic)
                {
                    _logger.LogInformation(
                        "User {UserId} changed public note of repeating state {RepeatingStateId}. Original note: {OriginalNote}.",
                        changeMadeByUserId, stateEntity.Id, stateEntity.NotePublic);
                    modified = true;
                }

                if (modification.NoteInternal != null && modification.NoteInternal != stateEntity.NoteInternal)
                {
                    _logger.LogInformation(
                        "User {UserId} changed internal note of repeating state {RepeatingStateId}. Original note: {OriginalNote}.",
                        changeMadeByUserId, stateEntity.Id, stateEntity.NoteInternal);
                    modified = true;
                }

                if (modified)
                {
                    if (modification.NotePublic == string.Empty)
                    {
                        stateEntity.NotePublic = null;
                    }
                    else if (modification.NotePublic != null)
                    {
                        stateEntity.NotePublic = modification.NotePublic;
                    }

                    if (modification.NoteInternal == string.Empty)
                    {
                        stateEntity.NoteInternal = null;
                    }
                    else if (modification.NoteInternal != null)
                    {
                        stateEntity.NoteInternal = modification.NoteInternal;
                    }
                }
            }
        }

        /// <inheritdoc />
        public async Task<StatePlanningResult> PlanState(NewState newState)
        {
            if (newState is null)
                throw new ArgumentNullException(nameof(newState));
            if (newState.Type == StateType.Closed)
            {
                throw new ArgumentException(
                    $"Cannot plan a closed state. Use {nameof(this.CloseNow)} to close the current state instead.",
                    nameof(newState));
            }

            bool startsNow;
            if (newState.Start.HasValue)
            {
                var nowRounded = DateTime.Now.RoundToMinutes();
                newState.Start = newState.Start.Value.RoundToMinutes();

                if (newState.Start < nowRounded)
                    throw new ArgumentException("Cannot plan a state in the past.", nameof(newState));

                startsNow = newState.Start.Value == nowRounded;
            }
            else
            {
                newState.Start = DateTime.Now.RoundToMinutes();
                startsNow = true;
            }

            // Ensure the user exists
            await this.EnsureUser(newState.MadeById);

            // Determine the end datetime.
            // The model may specify either one of PlannedEnd or FollowingStateId or nothing.
            if (newState.PlannedEnd.HasValue && newState.FollowingStateId.HasValue)
                throw new ArgumentException("Cannot specify both the planned end and following state ID.");

            // Lock
            await EnsureLock();
            try
            {
                await _unitOfWork.BeginTransaction();
                var result = await this.CheckAndCreatePlannedState(newState);
                await _unitOfWork.CommitTransaction();
                await _statePlannerService.NotifyPlanChanged();

                if (!result.HasOverlappingStates && startsNow)
                {
                    TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, _) =>
                    {
                        var transitionService = services.GetRequiredService<IStateTransitionService>();
                        if (result.ModifiedPreviousStateId.HasValue)
                        {
                            await transitionService.TriggerStateEnd(result.ModifiedPreviousStateId.Value,
                                result.TargetStateId);
                        }

                        await transitionService.TriggerStateStart(result.TargetStateId, result.ModifiedPreviousStateId);
                    });
                }

                return result;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
            finally
            {
                StatePlanningSemaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task RemovePlannedState(int id)
        {
            var stateEntity = await _stateRepository.Get(id);
            if (stateEntity is null)
                throw new StateNotFoundException(id);

            if (stateEntity.Start <= DateTime.Now.RoundToMinutes())
                throw new StateReadOnlyException(id);

            _logger.LogInformation("Removing state {Id}.", id);

            await EnsureLock();

            try
            {
                await _stateRepository.Delete(stateEntity);
                await _unitOfWork.SaveChanges();
                await _statePlannerService.NotifyPlanChanged();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot remove planned state {StateId}.", id);
                throw new StatePlanningException("Cannot remove the specified planned state.", e, id);
            }
            finally
            {
                StatePlanningSemaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task<StatePlanningResult> ModifyState(StateModification modification, int changeMadeByUserId)
        {
            if (modification is null)
                throw new ArgumentNullException(nameof(modification));

            PlannedState modifiedStateEntity;
            StateRelativeTime relativeTime;
            var now = DateTime.Now.RoundToMinutes();

            // Determine state entity to modify
            if (!modification.StateId.HasValue)
            {
                // We're changing the current state
                relativeTime = StateRelativeTime.Current;

                // Check that a state is currently active
                modifiedStateEntity = await _stateRepository.GetCurrent();
                if (modifiedStateEntity is null)
                {
                    throw new StateNotFoundException(
                        "Cannot modify the current state because no state is active at the moment.");
                }
            }
            else
            {
                // We're changing a specified state

                // Check that the specified state exists
                modifiedStateEntity = await _stateRepository.Get(modification.StateId.Value);
                if (modifiedStateEntity is null)
                    throw new StateNotFoundException(modification.StateId.Value);

                // Determine whether we're modifying a past, the current or a future state
                if (modifiedStateEntity.Start <= now && modifiedStateEntity.PlannedEnd > now)
                    relativeTime = StateRelativeTime.Current;
                else if (modifiedStateEntity.PlannedEnd <= now)
                    relativeTime = StateRelativeTime.Past;
                else
                    relativeTime = StateRelativeTime.Planned;
            }

            // Check mutability
            switch (relativeTime)
            {
                case StateRelativeTime.Past:
                    if (modification.Start.HasValue || modification.PlannedEnd.HasValue
                                                    || modification.NotePublic != null)
                        throw new InvalidOperationException("Cannot modify required properties of past states.");
                    break;
                case StateRelativeTime.Current:
                    if (modification.Start.HasValue)
                        throw new InvalidOperationException("Cannot modify the start time of the current state.");
                    break;
                case StateRelativeTime.Planned:
                    break;
            }

            // Keep the original state for the trigger handler
            var originalState = await this.GetState(modifiedStateEntity.Id);

            // Check user IDs (and apply potential MadeById modifications)
            await CheckAndModifyUserReferences();

            // Apply note modifications
            ModifyNotes();

            // Past states handling is easier as almost nothing can be changed
            if (relativeTime == StateRelativeTime.Past)
            {
                try
                {
                    await _unitOfWork.SaveChanges();
                    return new StatePlanningResult()
                    {
                        TargetStateId = modifiedStateEntity.Id,
                        TargetStatePlannedEnd = modifiedStateEntity.PlannedEnd
                    };
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Cannot modify past state {StateId}.", modifiedStateEntity.Id);
                    throw new StatePlanningException("Cannot modify the specified past state.", e,
                        modifiedStateEntity.Id);
                }
            }

            // Check dates
            var startDate = (modification.Start ?? modifiedStateEntity.Start).RoundToMinutes();
            var endDate = (modification.PlannedEnd ?? modifiedStateEntity.PlannedEnd).RoundToMinutes();

            if (modification.Start.HasValue && startDate < now)
                throw new InvalidOperationException("Cannot change the start date of a state to the past.");

            if (endDate <= startDate)
            {
                throw new InvalidOperationException(
                    "The new end date and time would be before the new start date and time.");
            }

            if (endDate <= now)
                throw new InvalidOperationException("The new end date would be in the past.");

            // If we're changing the planned start, check if there isn't a previous state linked to this one
            // If there is, change its NextStateId to null

            await EnsureLock();
            try
            {
                if (startDate != modifiedStateEntity.Start)
                {
                    _logger.LogInformation(
                        "User {UserId} is changing StartDate of State {Id} from {OriginalStartDate} to {NewStartDate}",
                        changeMadeByUserId, modifiedStateEntity.Id, modifiedStateEntity.Start, startDate);

                    var previousStateEntity = await _stateRepository.GetPreviousFor(modifiedStateEntity.Id);
                    if (previousStateEntity != null)
                    {
                        previousStateEntity.NextPlannedStateId = null;
                    }
                }

                var isProlonged = (modification.Start.HasValue &&
                                   modification.Start.Value < modifiedStateEntity.Start)
                                  || (modification.PlannedEnd.HasValue &&
                                      modification.PlannedEnd.Value > modifiedStateEntity.PlannedEnd);

                // If we're changing the planned end, check the modified state's NextStateId
                // If there is one, it's either a conflict (if prolonging) or it should be set to null (if shortening)
                if (endDate != modifiedStateEntity.PlannedEnd && modifiedStateEntity.NextPlannedStateId.HasValue)
                {
                    _logger.LogInformation(
                        "User {UserId} is changing PlannedEnd of State {Id} from {OriginalPlannedEnd} to {NewPlannedEnd}",
                        changeMadeByUserId, modifiedStateEntity.Id, modifiedStateEntity.PlannedEnd, endDate);

                    if (isProlonged)
                    {
                        await _unitOfWork.ClearTrackedChanges();
                        return new StatePlanningResult()
                        {
                            TargetStateId = modifiedStateEntity.Id,
                            TargetStatePlannedEnd = modifiedStateEntity.PlannedEnd,
                            OverlappingStatesIds = new List<int> { modifiedStateEntity.NextPlannedStateId.Value }
                        };
                    }
                    else
                    {
                        modifiedStateEntity.NextPlannedStateId = null;
                    }
                }

                int? previousStateId = null;
                DateTime? previousStatePlannedEnd = null;

                // If we're prolonging the state, check for overlaps
                if (isProlonged)
                {
                    // Check for blocking overlaps (already planned states that would start during the modified one)
                    var overlappingStateEntities = _stateRepository.GetStartingBetween(startDate,
                        endDate);

                    var overlappingIds = new List<int>();
                    await foreach (var overlappingStateEntity in overlappingStateEntities)
                    {
                        if (overlappingStateEntity.Id == modifiedStateEntity.Id)
                            continue;

                        // If one of the 'overlapping' states actually begins at the exact same time when the planned state ends
                        // it will be the modified state's new following state
                        if (overlappingStateEntity.Start == endDate)
                            modifiedStateEntity.NextPlannedStateId = overlappingStateEntity.Id;
                        else
                            overlappingIds.Add(overlappingStateEntity.Id);
                    }

                    if (overlappingIds.Count > 0)
                    {
                        await _unitOfWork.ClearTrackedChanges();
                        return new StatePlanningResult()
                        {
                            TargetStateId = modifiedStateEntity.Id,
                            TargetStatePlannedEnd = modifiedStateEntity.PlannedEnd,
                            OverlappingStatesIds = overlappingIds
                        };
                    }

                    var previousStateEntity = await _stateRepository.GetCurrent(startDate, true);
                    if (previousStateEntity != null && previousStateEntity.Id != modifiedStateEntity.Id)
                    {
                        previousStateId = previousStateEntity.NextPlannedStateId = modifiedStateEntity.Id;
                        previousStatePlannedEnd = previousStateEntity.PlannedEnd = startDate;
                    }
                }

                // Save the new details
                modifiedStateEntity.Start = startDate;
                modifiedStateEntity.PlannedEnd = endDate;

                try
                {
                    await _unitOfWork.SaveChanges();
                    await _statePlannerService.NotifyPlanChanged();

                    TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, _) =>
                    {
                        var transitionService = services.GetRequiredService<IStateTransitionService>();
                        await transitionService.TriggerStateModification(originalState);
                    });

                    return new StatePlanningResult()
                    {
                        TargetStateId = modifiedStateEntity.Id,
                        TargetStatePlannedEnd = modifiedStateEntity.PlannedEnd,
                        ModifiedPreviousStateId = previousStateId,
                        ModifiedPreviousStatePlannedEnd = previousStatePlannedEnd
                    };
                }
                catch (Exception e)
                {
                    await _unitOfWork.ClearTrackedChanges();
                    _logger.LogError(e, "Cannot modify state {StateId}.", modifiedStateEntity.Id);
                    throw new StatePlanningException("Cannot modify the state.", e, modifiedStateEntity.Id);
                }
            }
            finally
            {
                StatePlanningSemaphore.Release();
            }

            async Task CheckAndModifyUserReferences()
            {
                var changeMadeByUserRoles = await _userService.GetUserRoles(changeMadeByUserId);
                if (changeMadeByUserRoles is null)
                    throw new UserNotFoundException(changeMadeByUserId);

                if (modification.MadeById.HasValue)
                {
                    if (!changeMadeByUserRoles.Any(r => r == AuthConstants.Admin))
                    {
                        throw new UserUnprivilegedException(changeMadeByUserId, AuthConstants.Admin,
                            $"change the {nameof(RepeatingState.MadeById)} attribute of states");
                    }

                    var newMadeByUser = await _userService.GetUser(modification.MadeById.Value);
                    if (newMadeByUser is null)
                        throw new UserNotFoundException(modification.MadeById.Value);

                    _logger.LogInformation(
                        "User {UserId} changed MadeById of state {StateId} from {OriginalMadeById} to {NewMadeById}.",
                        changeMadeByUserId, modifiedStateEntity.Id, modifiedStateEntity.MadeById,
                        modification.MadeById.Value);

                    modifiedStateEntity.MadeById = modification.MadeById.Value;
                }
                else
                {
                    var newMadeByUser = await _userService.GetUser(changeMadeByUserId);
                    if (newMadeByUser is null)
                        throw new UserNotFoundException(changeMadeByUserId);

                    _logger.LogInformation(
                        "Changing MadeById of state {StateId} from {OriginalMadeById} to {NewMadeById}.",
                        modifiedStateEntity.Id, modifiedStateEntity.MadeById, changeMadeByUserId);

                    modifiedStateEntity.MadeById = changeMadeByUserId;
                }
            }

            void ModifyNotes()
            {
                if (modification.NotePublic != null && modification.NotePublic != modifiedStateEntity.NotePublic)
                {
                    _logger.LogInformation(
                        "User {UserId} changed public note of state {StateId}. Original note: {OriginalNote}.",
                        changeMadeByUserId, modifiedStateEntity.Id, modifiedStateEntity.NotePublic);
                }

                if (modification.NoteInternal != null &&
                    modification.NoteInternal != modifiedStateEntity.NoteInternal)
                {
                    _logger.LogInformation(
                        "User {UserId} changed internal note of state {StateId}. Original note: {OriginalNote}.",
                        changeMadeByUserId, modifiedStateEntity.Id, modifiedStateEntity.NoteInternal);
                }

                if (modification.NotePublic == string.Empty)
                {
                    modifiedStateEntity.NotePublic = null;
                }
                else if (modification.NotePublic != null)
                {
                    modifiedStateEntity.NotePublic = modification.NotePublic;
                }

                if (modification.NoteInternal == string.Empty)
                {
                    modifiedStateEntity.NoteInternal = null;
                }
                else if (modification.NoteInternal != null)
                {
                    modifiedStateEntity.NoteInternal = modification.NoteInternal;
                }
            }
        }

        /// <inheritdoc />
        public async Task CloseNow(int closedByUserId)
        {
            // Ensure the user exists
            await this.EnsureUser(closedByUserId);

            _logger.LogInformation("User {UserId} is closing the current state.", closedByUserId);

            await EnsureLock();

            var currentStateId = -1;
            try
            {
                var currentStateEntity = await _stateRepository.GetCurrent();
                if (currentStateEntity is null)
                    throw new StateNotFoundException("No state is currently active.");

                currentStateEntity.Ended = DateTime.Now;
                currentStateEntity.ClosedById = closedByUserId;
                currentStateEntity.NextPlannedStateId = null;

                currentStateId = currentStateEntity.Id;
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e) when (e is not StateNotFoundException)
            {
                _logger.LogError(e, "Cannot close the current state.");
                throw new StatePlanningException("Cannot close the current state.", e, currentStateId);
            }
            finally
            {
                StatePlanningSemaphore.Release();
            }

            // Cancel the current state end triggers in the background service
            await _statePlannerService.NotifyPlanChanged();

            // ... and run the triggers manually (in the background)
            TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, _) =>
            {
                var transitionService = services.GetRequiredService<IStateTransitionService>();
                await transitionService.TriggerStateEnd(currentStateId, null);
            });
        }

        /// <summary>
        /// A helper routine for <see cref="PlanState"/> and <see cref="PlanStatesForRepeatingState"/>.
        /// Performs overlap checks for a new planned state, adjusts adjacent states and creates a database
        /// entry for the new state.
        /// </summary>
        /// <remarks>
        /// Calls to this method should be made inside of a database transaction and the state planning lock.
        /// </remarks>
        private async Task<StatePlanningResult> CheckAndCreatePlannedState(NewState newState)
        {
            if (newState.FollowingStateId.HasValue)
            {
                // Following state was specified: get the state and use its start date as the new state's start date
                var followingStateEntity = await _stateRepository.Get(newState.FollowingStateId.Value);
                if (followingStateEntity is null)
                    throw new StateNotFoundException(newState.FollowingStateId.Value);

                newState.PlannedEnd = followingStateEntity.Start;
            }
            else if (!newState.PlannedEnd.HasValue)
            {
                // Neither planned end nor following state was specified; get the first state that comes after
                // the specified start date and use it as the following state
                var nextStateEntity = await _stateRepository.GetNearest(after: newState.Start);
                if (nextStateEntity is null)
                {
                    throw new StateNotFoundException(
                        "No state after the specified start date is planned. Planned end must be specified.");
                }

                newState.PlannedEnd = nextStateEntity.Start;
                newState.FollowingStateId = nextStateEntity.Id;
            }

            newState.PlannedEnd = newState.PlannedEnd.Value.RoundToMinutes();

            if (newState.PlannedEnd <= newState.Start)
                throw new InvalidOperationException("The state's planned end must come after its start.");

            // Check for blocking overlaps (already planned states that would start during the newly created one)
            var overlappingStateEntities = _stateRepository.GetStartingBetween(newState.Start.Value,
                newState.PlannedEnd.Value);

            int? followingStateId = null;
            var overlappingIds = new List<int>();
            await foreach (var overlappingStateEntity in overlappingStateEntities)
            {
                // If one of the 'overlapping' states actually begins at the exact same time when the planned state ends
                // it will be the new state's following state
                if (overlappingStateEntity.Start == newState.PlannedEnd.Value)
                    followingStateId = overlappingStateEntity.Id;
                else
                    overlappingIds.Add(overlappingStateEntity.Id);
            }

            if (overlappingIds.Count > 0)
                return new StatePlanningResult() { OverlappingStatesIds = overlappingIds };

            // TODO: map using automapper?
            var newStateEntity = new PlannedState()
            {
                MadeById = newState.MadeById,
                NoteInternal = newState.NoteInternal,
                NotePublic = newState.NotePublic,
                Start = newState.Start.Value,
                PlannedEnd = newState.PlannedEnd.Value,
                State = _mapper.Map<KachnaOnline.Data.Entities.ClubStates.StateType>(newState.Type),
                NextPlannedStateId = followingStateId,
                RepeatingStateId = newState.RepeatingStateId
            };

            // Check for non-blocking overlaps (the new state starts in the middle of another, already planned one)
            var existingStateEntity = await _stateRepository.GetCurrent(newState.Start, true);

            await _stateRepository.Add(newStateEntity);

            try
            {
                // First we have to create the new entity
                await _unitOfWork.SaveChanges();

                if (existingStateEntity != null)
                {
                    // ... and then relink the existing one
                    existingStateEntity.PlannedEnd = newState.Start.Value;
                    existingStateEntity.NextPlannedStateId = newStateEntity.Id;
                    await _unitOfWork.SaveChanges();
                }

                return new StatePlanningResult()
                {
                    TargetStateId = newStateEntity.Id,
                    TargetStatePlannedEnd = newStateEntity.PlannedEnd,
                    ModifiedPreviousStateId = existingStateEntity?.Id,
                    ModifiedPreviousStatePlannedEnd = existingStateEntity?.PlannedEnd
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot save a new state.");
                throw new StatePlanningException("Cannot save the new state.", e);
            }
        }

        /// <summary>
        /// A helper routine for <see cref="MakeRepeatingState"/> and <see cref="ModifyRepeatingState"/>.
        /// Creates <see cref="PlannedState"/> objects for a repeating state, starting from <param name="dateFrom"></param>,
        /// and attempts to plan them using <see cref="CheckAndCreatePlannedState"/>. This is encapsulated in a database transaction
        /// which is rollbacked in case of an error.
        /// </summary>
        /// <remarks>
        /// Calls to this method should be made inside of the state planning lock.
        /// </remarks>
        private async Task<RepeatingStatePlanningResult> PlanStatesForRepeatingState(
            RepeatingStateEntity repeatingStateEntity, DateTime dateFrom)
        {
            var dateTo = repeatingStateEntity.EffectiveTo;
            var timeFrom = repeatingStateEntity.TimeFrom;
            var timeTo = repeatingStateEntity.TimeTo;

            try
            {
                await _unitOfWork.BeginTransaction();

                var overlappingStates = new List<State>();
                for (var date = dateFrom; date <= dateTo; date = date.AddDays(7))
                {
                    var newState = new NewState()
                    {
                        Start = date + timeFrom,
                        Type = _mapper.Map<StateType>(repeatingStateEntity.State),
                        NoteInternal = repeatingStateEntity.NoteInternal,
                        NotePublic = repeatingStateEntity.NotePublic,
                        PlannedEnd = date + timeTo,
                        MadeById = repeatingStateEntity.MadeById,
                        RepeatingStateId = repeatingStateEntity.Id
                    };

                    var result = await this.CheckAndCreatePlannedState(newState);
                    if (result.HasOverlappingStates)
                    {
                        foreach (var overlappingStateId in result.OverlappingStatesIds)
                        {
                            overlappingStates.Add(await this.GetState(overlappingStateId));
                        }
                    }
                }

                await _unitOfWork.CommitTransaction();
                await _statePlannerService.NotifyPlanChanged();
                return new RepeatingStatePlanningResult()
                {
                    OverlappingStates = overlappingStates,
                    TargetRepeatingStateId = repeatingStateEntity.Id
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot plan a new repeating state, rollbacking changes.");
                await _unitOfWork.RollbackTransaction();
                await _statePlannerService.NotifyPlanChanged();
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UnlinkStateFromEvent(int stateId)
        {
            var state = await _stateRepository.Get(stateId);
            if (state is null)
                throw new StateNotFoundException(stateId);
            if (state.Ended is not null || state.Start < DateTime.Now)
                throw new StateReadOnlyException(stateId);
            if (state.AssociatedEventId is null)
                throw new StateNotAssociatedToEventException(stateId);

            state.AssociatedEventId = null;
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Cannot unlink associated event from state with ID {stateId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw;
            }
        }
    }
}
