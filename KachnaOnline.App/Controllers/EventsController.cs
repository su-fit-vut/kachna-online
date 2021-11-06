// EventsController.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Events;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("events")]
    [Authorize(Roles = RoleConstants.EventsManager)]
    public class EventsController : ControllerBase
    {
        private readonly EventsFacade _eventsFacade;

        public EventsController(EventsFacade eventsFacade)
        {
            _eventsFacade = eventsFacade;
        }

        /// <summary>
        /// Returns the list of events being held at the specified time.
        /// </summary>
        /// <param name="at">Specifies the date and time when the events have to be held.</param>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Events.EventDto"/> corresponding to the <paramref name="at"/> date and time.</returns>
        /// <response code="200">The list of events.</response>
        [AllowAnonymous]
        [HttpGet("at/{at}")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(DateTime at)
        {
            var events = await _eventsFacade.GetEvents(at);
            return new ActionResult<IEnumerable<EventDto>>(events);
        }

        /// <summary>
        /// Returns an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to return.</param>
        /// <returns>An <see cref="KachnaOnline.Dto.Events.EventDto"/> of a game corresponding to ID <paramref name="eventId"/>.</returns>
        /// <response code="200">The event.</response>
        /// <response code="404">No such event exists.</response>
        [AllowAnonymous]
        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EventDto>> GetEvent(int eventId)
        {
            try
            {
                return await _eventsFacade.GetEvent(eventId);
            }
            catch (EventNotFoundException)
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Returns the list of current events.
        /// </summary>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Events.EventDto"/>.</returns>
        /// <response code="200">The list of current events.</response>
        [AllowAnonymous]
        [HttpGet("current")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetCurrentEvents()
        {
            var events = await _eventsFacade.GetCurrentEvents();
            return new ActionResult<IEnumerable<EventDto>>(events);
        }

        /// <summary>
        /// Returns the list of next planned events.
        /// </summary>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Events.EventDto"/> of the next planned events.</returns>
        /// <response code="200">The list of the next planned events.</response>
        [AllowAnonymous]
        [HttpGet("next")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetNextPlannedEvents()
        {
            var events = await _eventsFacade.GetNextPlannedEvents();
            return new ActionResult<IEnumerable<EventDto>>(events);
        }

        /// <summary>
        /// Returns the list of the events planned in the specified timespan.
        /// </summary>
        /// <returns>A List of <see cref="KachnaOnline.Dto.Events.EventDto"/> of the events planned in the timespan
        /// specified by <paramref name="from"/> and <paramref name="to"/>.</returns>
        /// <response code="200">The list of the events.</response>
        /// <response code="422">Wrong arguments.</response>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
        [ProducesResponseType(422)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(
            DateTime? from = null,
            DateTime? to = null)
        {
            try
            {
                var events = await _eventsFacade.GetEvents(from, to);
                return new ActionResult<IEnumerable<EventDto>>(events);
            }
            catch (ArgumentException)
            {
                return this.Problem(
                    title: "Wrong arguments",
                    detail: "Got wrong arguments in the request.",
                    statusCode: 422);
            }
        }

        /// <summary>
        /// Plans a new event.
        /// </summary>
        /// <param name="newEvent"><see ref="KachnaOnline.Dto.Events.NewEventDto"/> to create.</param>
        /// <returns>A planned <see cref="KachnaOnline.Dto.Events.EventDto"/> if the creation succeeded.</returns>
        /// <response code="201">The new event.</response>
        /// <response code="422">A user with the given ID does not exist.</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        public async Task<ActionResult<ManagerEventDto>> PlanEvent(BaseEventDto newEvent)
        {
            try
            {
                return await _eventsFacade.PlanEvent(newEvent);
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntity();
            }
            catch (ArgumentException)
            {
                return this.UnprocessableEntity();
            }
            catch (EventManipulationFailedException)
            {
                return this.Problem(
                    title: "Creation failed",
                    detail: "Creation of a new event has failed.",
                    statusCode: 500);
            }
        }

        /// <summary>
        /// Modifies an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to update.</param>
        /// <param name="baseEvent"><see ref="KachnaOnline.Dto.Events.BaseEventDto"/> representing the new state.</param>
        /// <response code="204">The event was updated.</response>
        /// <response code="404">The event with the given ID does not exist.</response>
        /// <response code="409">The event from the past cannot be modified.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpPut("{eventId}")]
        public async Task<ActionResult> ModifyEvent(int eventId, BaseEventDto baseEvent)
        {
            try
            {
                await _eventsFacade.ModifyEvent(eventId, baseEvent);
                return this.NoContent();
            }
            catch (NotAnEventsManagerException)
            {
                return this.Forbid();
            }
            catch (EventNotFoundException)
            {
                return this.NotFound();
            }
            catch (EventReadOnlyException)
            {
                return this.Conflict();
            }
            catch (EventManipulationFailedException)
            {
                return this.Problem(
                    title: "Modification failed",
                    detail: $"Modification of the event failed.",
                    statusCode: 500);
            }
        }

        /// <summary>
        /// Removes an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to delete.</param>
        /// <response code="204">The event was deleted.</response>
        /// <response code="404">The event with the given ID does not exist.</response>
        /// <response code="409">The event from the past cannot be removed.</response>
        [HttpDelete("{eventId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult> RemoveEvent(int eventId)
        {
            try
            {
                await _eventsFacade.RemoveEvent(eventId);
                return this.NoContent();
            }
            catch (NotAnEventsManagerException)
            {
                return this.Forbid();
            }
            catch (EventReadOnlyException)
            {
                return this.Conflict();
            }
            catch (EventNotFoundException)
            {
                return this.NotFound();
            }
            catch (EventManipulationFailedException)
            {
                return this.Problem(
                    title: "Removal failed",
                    detail: "Removal of an event failed.",
                    statusCode: 500);
            }
        }
    }
}
