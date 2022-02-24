using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Exceptions.Events;
using KachnaOnline.Business.Models.ClubStates;
using KachnaOnline.Business.Models.Events;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.ClubStates;
using KachnaOnline.Dto.Events;
using Microsoft.AspNetCore.Http;

namespace KachnaOnline.Business.Facades
{
    public class EventsFacade
    {
        private readonly IEventsService _eventsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public EventsFacade(IEventsService eventsService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _eventsService = eventsService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private IEnumerable<EventDto> MapEvents(ICollection<Event> events)
        {
            if (events is not { Count: >0 })
                return new List<EventDto>();

            if (this.IsUserEventsManager())
                return _mapper.Map<List<ManagerEventDto>>(events);

            return _mapper.Map<List<EventDto>>(events);
        }

        private EventDto MapEvent(Event @event)
        {
            if (this.IsUserEventsManager())
                return _mapper.Map<ManagerEventDto>(@event);

            return _mapper.Map<EventDto>(@event);
        }

        private EventWithLinkedStatesDto MapEventWithLinkedStates(EventWithLinkedStates @event)
        {
            if (this.IsUserEventsManager())
            {
                return _mapper.Map<ManagerEventWithLinkedStatesDto>(@event);
            }

            return _mapper.Map<EventWithLinkedStatesDto>(@event);
        }

        private IEnumerable<StateDto> MapPlannedStates(ICollection<State> states)
        {
            return _mapper.Map<List<StateDto>>(states) ?? Enumerable.Empty<StateDto>();
        }

        /// <summary>
        /// Checks whether the current user is an events manager.
        /// </summary>
        /// <returns>True if the current user is the events manager, otherwise false.</returns>
        private bool IsUserEventsManager()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user != null && user.IsInRole(AuthConstants.EventsManager);
        }

        /// <summary>
        /// Get current user Id
        /// </summary>
        /// <exception cref="InvalidOperationException"> thrown when the current user could not be found.</exception>
        private int CurrentUserId =>
            int.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(IdentityConstants.IdClaim) ??
                      throw new InvalidOperationException("No valid user found in the current request."));


        /// <summary>
        /// Returns a list of current events.
        /// </summary>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Events.EventDto"/> of current events or an empty list if no there is no event currently.
        /// The list usually contains only one event, more in case multiple next events start at the same time.</returns>
        public async Task<IEnumerable<EventDto>> GetCurrentEvents()
        {
            var events = await _eventsService.GetCurrentEvents();
            return this.MapEvents(events);
        }

        /// <summary>
        /// Returns a list of events that are held at the date and time specified by <paramref name="at"/>.
        /// </summary>
        /// <param name="at">Date and time to look for held events.</param>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Events.EventDto"/> of events or an empty list if there is no event at the specified date and time.</returns>
        public async Task<IEnumerable<EventDto>> GetEvents(DateTime at)
        {
            var events = await _eventsService.GetEvents(at);
            return this.MapEvents(events);
        }

        /// <summary>
        /// Returns a list of events starting in the future, optionally only the ones starting in the timespan specified by <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">If not null, only returns events with their from date being after or equal to the specified value.</param>
        /// <param name="to">If not null, only returns events with their to date being before or equal to the specified value.</param>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Events.EventDto"/> or an empty list if no there is no event planned.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="from"/> or <paramref name="to"/> are invalid.</exception>
        public async Task<IEnumerable<EventDto>> GetEvents(DateTime? from = null, DateTime? to = null)
        {
            var events = await _eventsService.GetEvents(from, to);
            return this.MapEvents(events);
        }

        /// <summary>
        /// Returns an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the <see cref="KachnaOnline.Dto.Events.EventDto"/> to return.</param>
        /// <param name="withLinkedStates">Whether to return linked states as well.</param>
        /// <returns>An <see cref="KachnaOnline.Dto.Events.EventDto"/> with the given ID.</returns>
        /// <exception cref="EventNotFoundException"> thrown when an event with the given
        /// <paramref name="eventId"/> does not exist.</exception>
        public async Task<EventDto> GetEvent(int eventId, bool withLinkedStates = false)
        {
            if (withLinkedStates)
            {
                return this.MapEventWithLinkedStates(await _eventsService.GetEventWithLinkedStates(eventId));
            }

            return this.MapEvent(await _eventsService.GetEvent(eventId));
        }

        /// <summary>
        /// Returns a list of next events.
        /// </summary>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Events.EventDto"/> planned as the next events or an empty list if no event is planned.
        /// The list usually contains only one event, more in case multiple next events start at the same time.</returns>
        public async Task<IEnumerable<EventDto>> GetNextPlannedEvents()
        {
            var events = await _eventsService.GetNextPlannedEvents();
            return this.MapEvents(events);
        }

        /// <summary>
        /// Plans a new event.
        /// </summary>
        /// <param name="newEvent">New event data to create the new event with.</param>
        /// <returns>Created event with its ID attribute filled.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newEvent"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newEvent"/> has invalid attributes.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when the board game cannot be created.
        /// This can be caused by a database error.</exception>
        /// <exception cref="UserNotFoundException">Thrown when a user with the ID assigned to the event does
        /// not exist.</exception>
        public async Task<ManagerEventDto> PlanEvent(BaseEventDto newEvent)
        {
            var newEventModel = _mapper.Map<NewEvent>(newEvent);
            newEventModel.MadeById = this.CurrentUserId;

            var createdEvent = await _eventsService.PlanEvent(newEventModel);

            return _mapper.Map<ManagerEventDto>(createdEvent);
        }

        /// <summary>
        /// Get conflicting states for an event specified by <paramref name="eventId"/>.
        /// </summary>
        /// <param name="eventId">The event ID to get the conflicting states for.</param>
        /// <returns>An enumerable of <see cref="StateDto"/> of conflicting states.</returns>
        /// <exception cref="EventNotFoundException">When the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        public async Task<IEnumerable<StateDto>> GetConflictingStatesForEvent(int eventId)
        {
            var conflictingPlannedStates = await _eventsService.GetConflictingStatesForEvent(eventId);
            return this.MapPlannedStates(conflictingPlannedStates);
        }

        /// <summary>
        /// Modifies an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to update.</param>
        /// <param name="baseEvent"><see ref="KachnaOnline.Dto.Events.BaseEventDto"/> representing the new state.</param>
        /// <exception cref="ArgumentNullException">Thrown when the passed <paramref name="modifiedEvent"/> model is null.</exception>
        /// <exception cref="EventNotFoundException">Thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        /// <exception cref="EventReadOnlyException">Thrown when event to be modified has already ended.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when event modification has failed.</exception>
        public async Task ModifyEvent(int eventId, BaseEventDto baseEvent)
        {
            await _eventsService.ModifyEvent(eventId, _mapper.Map<ModifiedEvent>(baseEvent));
        }

        /// <summary>
        /// Remove an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to remove.</param>
        /// <returns>A collection of <see cref="StateDto"/> of states linked to the event at the time of deletion.</returns>
        /// <exception cref="EventNotFoundException">Thrown when the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when the event cannot be deleted.</exception>
        /// <exception cref="EventReadOnlyException">Thrown when event to be removed has already ended.</exception>
        public async Task<IEnumerable<StateDto>> RemoveEvent(int eventId)
        {
            return this.MapPlannedStates(await _eventsService.RemoveEvent(eventId));
        }

        /// <summary>
        /// Sets (overrides) the current linked states to the event specified by <paramref name="eventId"/> with <paramref name="plannedStatesToLink"/>.
        /// </summary>
        /// <param name="eventId">Event to set linked planned states for.</param>
        /// <param name="plannedStatesToLink">A <see cref="PlannedStatesToLinkDto"/> of states to link to the event.</param>
        /// <exception cref="EventNotFoundException">Thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when linked planned states cannot be unlinked from the event.</exception>
        /// <exception cref="StateReadOnlyException">Thrown when planned state to be unlinked from the event has already started or ended.</exception>
        /// <exception cref="EventReadOnlyException">Thrown when event to unlinked linked states from has already ended.</exception>
        /// <exception cref="LinkingStateToEventException">Thrown when linking planned states to the event is not possible.</exception>
        /// <exception cref="StateNotFoundException">Thrown when planned state to be linked to the event has not been found.</exception>
        public async Task SetLinkedPlannedStatesForEvent(int eventId, PlannedStatesToLinkDto plannedStatesToLink)
        {
            await _eventsService.SetLinkedPlannedStatesForEvent(eventId, plannedStatesToLink.PlannedStateIds);
        }

        /// <summary>
        /// Clears (unlinks) the current linked states from the event specified by <paramref name="eventId"/>.
        /// </summary>
        /// <param name="eventId">Event to unlink linked planned states from.</param>
        /// <exception cref="EventNotFoundException">Thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when linked planned states cannot be unlinked from the event.</exception>
        /// <exception cref="StateReadOnlyException">Thrown when planned state to be unlinked from the event has already started or ended.</exception>
        /// <exception cref="EventReadOnlyException">Thrown when event to unlinked linked states from has already ended.</exception>
        public async Task ClearLinkedPlannedStatesForEvent(int eventId)
        {
            await _eventsService.ClearLinkedPlannedStatesForEvent(eventId);
        }

        /// <summary>
        /// Link states to event specified by <paramref name="eventId"/>
        /// </summary>
        /// <param name="eventId">ID of the event to link states to.</param>
        /// <param name="plannedStatesToLink">A list of planned state IDs to link to the event specified by <paramref name="eventId"/>.</param>
        /// <exception cref="EventNotFoundException">Thrown when an event with the given <paramref name="eventId"/>
        /// does not exist.</exception>
        /// <exception cref="EventManipulationFailedException">Thrown when planned states cannot be linked to the event.</exception>
        /// <exception cref="LinkingStateToEventException">Thrown when linking planned states to the event is not possible.</exception>
        /// <exception cref="StateReadOnlyException">Thrown when planned state to be linked to the event has already started or ended.</exception>
        /// <exception cref="StateNotFoundException">Thrown when planned state to be linked to the event has not been found.</exception>
        /// <exception cref="EventReadOnlyException">Thrown when event to be linked to has already ended.</exception>
        public async Task LinkPlannedStatesToEvent(int eventId, PlannedStatesToLinkDto plannedStatesToLink)
        {
            await _eventsService.LinkPlannedStatesToEvent(eventId, plannedStatesToLink.PlannedStateIds);
        }

        /// <summary>
        /// Get states linked to the event.
        /// </summary>
        /// <param name="eventId">The event to get the linked states for.</param>
        /// <exception cref="EventNotFoundException">Thrown when the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        public async Task<IEnumerable<StateDto>> GetLinkedStatesForEvent(int eventId)
        {
            return this.MapPlannedStates(await _eventsService.GetLinkedStates(eventId));
        }
    }
}
