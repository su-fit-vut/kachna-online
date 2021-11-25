// EventsFacade.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions.Events;
using KachnaOnline.Business.Models.Events;
using KachnaOnline.Business.Services.Abstractions;
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
        /// <returns>An <see cref="KachnaOnline.Dto.Events.EventDto"/> with the given ID.</returns>
        /// <exception cref="EventNotFoundException"> thrown when an event with the given
        /// <paramref name="eventId"/> does not exist.</exception>
        public async Task<EventDto> GetEvent(int eventId)
        {
            var returnedEvent = await _eventsService.GetEvent(eventId);
            return this.MapEvent(returnedEvent);
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
        /// <exception cref="EventNotFoundException">When the event with the given <paramref name="eventId"/> does not
        /// exist.</exception>
        /// <exception cref="EventManipulationFailedException">When the event cannot be deleted.</exception>
        public async Task RemoveEvent(int eventId)
        {
            await _eventsService.RemoveEvent(eventId);
        }
    }
}
