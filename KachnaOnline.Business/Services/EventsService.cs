using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Exceptions.Events;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Models.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EventEntity = KachnaOnline.Data.Entities.Events.Event;
using StateType = KachnaOnline.Data.Entities.ClubStates.StateType;

namespace KachnaOnline.Business.Services
{
    public class EventsService : IEventsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EventsService> _logger;
        private readonly IEventsRepository _eventsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlannedStatesRepository _plannedStatesRepository;
        private readonly IOptionsMonitor<EventsOptions> _eventsOptionsMonitor;

        public EventsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EventsService> logger,
            IOptionsMonitor<EventsOptions> eventsOptionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _eventsRepository = _unitOfWork.Events;
            _userRepository = _unitOfWork.Users;
            _plannedStatesRepository = _unitOfWork.PlannedStates;
            _eventsOptionsMonitor = eventsOptionsMonitor;
        }

        /// <inheritdoc />
        public async Task<ICollection<Event>> GetCurrentEvents()
        {
            var eventEntities = _eventsRepository.GetCurrent();
            var resultList = new List<Event>();
            await foreach (var eventEntity in eventEntities)
            {
                resultList.Add(_mapper.Map<Event>(eventEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<Event> GetEvent(int eventId)
        {
            var eventEntity = await _eventsRepository.Get(eventId);

            if (eventEntity is null)
                throw new EventNotFoundException();

            return _mapper.Map<Event>(eventEntity);
        }

        /// <inheritDoc />
        public async Task<EventWithLinkedStates> GetEventWithLinkedStates(int eventId)
        {
            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            return _mapper.Map<EventWithLinkedStates>(eventEntity);
        }

        /// <inheritdoc />
        public async Task<ICollection<Event>> GetNextPlannedEvents()
        {
            var eventEntities = _eventsRepository.GetNearest();
            var resultList = new List<Event>();

            try
            {
                await foreach (var eventEntity in eventEntities)
                {
                    resultList.Add(_mapper.Map<Event>(eventEntity));
                }
            }
            catch (InvalidOperationException)
            {
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<ICollection<Event>> GetEvents(DateTime? from = null, DateTime? to = null)
        {
            from ??= DateTime.Now;
            to ??= from.Value.AddDays(_eventsOptionsMonitor.CurrentValue.QueryDaysTimeSpan);

            if (to < from)
            {
                throw new ArgumentException(
                    $"The {nameof(to)} argument must not be a datetime before {nameof(@from)}.");
            }

            if (to - from > TimeSpan.FromDays(_eventsOptionsMonitor.CurrentValue.QueryDaysTimeSpan))
            {
                throw new ArgumentException(
                    $"The maximum time span to get events for is {_eventsOptionsMonitor.CurrentValue.QueryDaysTimeSpan} days.");
            }

            var eventEntities = _eventsRepository.GetStartingBetween(
                from.Value.RoundToMinutes(), to.Value.RoundToMinutes());

            var resultList = new List<Event>();
            await foreach (var eventEntity in eventEntities)
            {
                resultList.Add(_mapper.Map<Event>(eventEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<ICollection<Event>> GetEvents(DateTime at)
        {
            var eventEntities = _eventsRepository.GetCurrent(at);
            var resultList = new List<Event>();
            await foreach (var eventEntity in eventEntities)
            {
                resultList.Add(_mapper.Map<Event>(eventEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<Event> PlanEvent(NewEvent newEvent)
        {
            if (newEvent is null)
                throw new ArgumentNullException(nameof(newEvent));

            newEvent.From = newEvent.From.RoundToMinutes();
            if (newEvent.From < DateTime.Now.RoundToMinutes())
                throw new ArgumentException("Cannot plan an event in the past.", nameof(newEvent));

            newEvent.To = newEvent.To.RoundToMinutes();
            if (newEvent.To < newEvent.From)
                throw new ArgumentException("Cannot plan an event which ends before it starts.", nameof(newEvent));

            if (await _userRepository.Get(newEvent.MadeById) is null)
                throw new UserNotFoundException(newEvent.MadeById);

            var newEventEntity = _mapper.Map<EventEntity>(newEvent);

            await _eventsRepository.Add(newEventEntity);

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Cannot save a new event.");
                await _unitOfWork.ClearTrackedChanges();
                throw new EventManipulationFailedException();
            }

            return _mapper.Map<Event>(newEventEntity);
        }

        public async Task<ICollection<State>> GetConflictingStatesForEvent(int eventId)
        {
            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            var conflictingPlannedStatesEntities =
                _plannedStatesRepository.GetConflictingStatesForEvent(eventEntity.From, eventEntity.To);

            var resultList = new List<State>();
            await foreach (var plannedStateEntity in conflictingPlannedStatesEntities)
            {
                resultList.Add(_mapper.Map<State>(plannedStateEntity));
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task ModifyEvent(int eventId, ModifiedEvent modifiedEvent)
        {
            if (modifiedEvent is null)
                throw new ArgumentNullException(nameof(modifiedEvent));

            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            if (eventEntity.To < DateTime.Now.RoundToMinutes())
                throw new EventReadOnlyException();

            _mapper.Map(modifiedEvent, eventEntity);
            try {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot modify the event with ID {eventId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw new EventManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task SetLinkedPlannedStatesForEvent(int eventId, IEnumerable<int> plannedStatesToLinkIds)
        {
            await this.ClearLinkedPlannedStatesForEvent(eventId);
            await this.LinkPlannedStatesToEvent(eventId, plannedStatesToLinkIds);
        }

        /// <inheritdoc />
        public async Task ClearLinkedPlannedStatesForEvent(int eventId)
        {
            var now = DateTime.Now.RoundToMinutes();

            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            if (eventEntity.To < now)
                throw new EventReadOnlyException();

            foreach (var stateEntity in eventEntity.LinkedPlannedStates)
            {
                if (stateEntity.Start < now || stateEntity.Ended is not null)
                    throw new StateReadOnlyException(stateEntity.Id);

                stateEntity.AssociatedEventId = null;
            }

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot clear linked planned states linked to the event with ID {eventId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw new EventManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task LinkPlannedStatesToEvent(int eventId, IEnumerable<int> plannedStatesToLinkIds)
        {
            var now = DateTime.Now.RoundToMinutes();

            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            if (eventEntity.To < now)
                throw new EventReadOnlyException();

            foreach (var stateId in plannedStatesToLinkIds)
            {
                var stateEntity = await _plannedStatesRepository.Get(stateId);
                if (stateEntity is null)
                    throw new StateNotFoundException();

                if (stateEntity.Start < now || stateEntity.Ended is not null)
                    throw new StateReadOnlyException(stateId);

                if (stateEntity.Start < eventEntity.From || stateEntity.PlannedEnd > eventEntity.To)
                    throw new LinkingStateToEventException("State is planned outside of the time span for the event.",
                        eventId, stateId);

                if (stateEntity.State == StateType.Private)
                    throw new LinkingStateToEventException("Private states cannot be linked to an event.");

                // Link the planned state to the event. Even if the planned state already has been linked to an event,
                // the former event is overwritten by this event.
                stateEntity.AssociatedEventId = eventId;
            }

            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot link planned states to the event with ID {eventId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw new EventManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<State>> GetLinkedStates(int eventId)
        {
            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            var resultList = new List<State>();
            foreach (var stateEntity in eventEntity.LinkedPlannedStates)
            {
                resultList.Add(_mapper.Map<State>(stateEntity));
            }

            return resultList;
        }


        /// <inheritdoc />
        public async Task<ICollection<State>> RemoveEvent(int eventId)
        {
            var eventEntity = await _eventsRepository.Get(eventId);
            if (eventEntity is null)
                throw new EventNotFoundException();

            if (eventEntity.To < DateTime.Now.RoundToMinutes())
                throw new EventReadOnlyException();

            // Get linked states.
            var statesList = eventEntity.LinkedPlannedStates.Select(
                plannedState => _mapper.Map<State>(plannedState)).ToList();

            // Remove event, set all linked planned states references to this event to null.
            await _eventsRepository.Delete(eventEntity);
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Cannot remove the event with ID {eventId}.");
                await _unitOfWork.ClearTrackedChanges();
                throw new EventManipulationFailedException();
            }

            // Return linked planned states with its reference to now nonexistent event.
            // Leave up to the manager to decide what to do with the remaining states.
            return statesList;
        }
    }
}
