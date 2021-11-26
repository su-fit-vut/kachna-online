// EventsFacade.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Constants;
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

            return this.MapEvent(await _eventsService.GetEventWithLinkedStates(eventId));
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
        /// <returns></returns>
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
        /// <exception cref="EventNotFoundException">When an event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        /// <exception cref="EventManipulationFailedException">When the event cannot be updated.</exception>
        public async Task ModifyEvent(int eventId, BaseEventDto baseEvent)
        {
            await _eventsService.ModifyEvent(eventId, _mapper.Map<ModifiedEvent>(baseEvent));
        }

        /// <summary>
        /// Remove an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to remove.</param>
        /// <returns>A collection of <see cref="StateDto"/> of states linked to the event at the time of deletion.</returns>
        /// <exception cref="EventNotFoundException">When the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        /// <exception cref="EventManipulationFailedException">When the event cannot be deleted.</exception>
        public async Task<IEnumerable<StateDto>> RemoveEvent(int eventId)
        {
            return this.MapPlannedStates(await _eventsService.RemoveEvent(eventId));
        }

        /// <summary>
        /// Sets (overrides) the current linked states to the event specified by <paramref name="eventId"/> with <paramref name="plannedStatesToLink"/>.
        /// </summary>
        /// <param name="eventId">Event to set linked planned states for.</param>
        /// <param name="plannedStatesToLink">A <see cref="PlannedStatesToLinkDto"/> of states to link to the event.</param>
        public async Task SetLinkedPlannedStatesForEvent(int eventId, PlannedStatesToLinkDto plannedStatesToLink)
        {
            await _eventsService.SetLinkedPlannedStatesForEvent(eventId, plannedStatesToLink.PlannedStateIds);
        }

        /// <summary>
        /// Clears (unlinks) the current linked states from the event specified by <paramref name="eventId"/>.
        /// </summary>
        /// <param name="eventId">Event to unlink linked planned states from.</param>
        public async Task ClearLinkedPlannedStatesForEvent(int eventId)
        {
            await _eventsService.ClearLinkedPlannedStatesForEvent(eventId);
        }

        /// <summary>
        /// Link states to event specified by <paramref name="eventId"/>
        /// </summary>
        /// <param name="eventId">ID of the event to link states to.</param>
        /// <param name="plannedStatesToLink">A list of planned state IDs to link to the event specified by <paramref name="eventId"/>.</param>
        public async Task LinkPlannedStatesToEvent(int eventId, PlannedStatesToLinkDto plannedStatesToLink)
        {
            await _eventsService.LinkPlannedStatesToEvent(eventId, plannedStatesToLink.PlannedStateIds);
        }

        /// <summary>
        /// Get states linked to the event.
        /// </summary>
        /// <param name="eventId">The event to get the linked states for.</param>
        public async Task<IEnumerable<StateDto>> GetLinkedStatesForEvent(int eventId)
        {
            return this.MapPlannedStates(await _eventsService.GetLinkedStates(eventId));
        }

        /// <summary>
        /// Unlinks the specified linked state from any event.
        /// </summary>
        /// <param name="stateId">ID of the planned state to be unlinked from any event.</param>
        public async Task UnlinkStateFromEvent(int stateId)
        {
            await _eventsService.UnlinkStateFromEvent(stateId);
        }
    }
}
