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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("events")]
    [Authorize(Roles = AuthConstants.EventsManager)]
    public class EventsController : ControllerBase
    {
        private readonly EventsFacade _eventsFacade;

        public EventsController(EventsFacade eventsFacade)
        {
            _eventsFacade = eventsFacade;
        }

        /// <summary>
        /// Returns a list of events being held at the given date and time.
        /// </summary>
        /// <param name="at">The date and time.</param>
        /// <returns>An enumerable of <see cref="EventDto"/> corresponding to the <paramref name="at"/> date and time.</returns>
        /// <response code="200">The list of events.</response>
        [AllowAnonymous]
        [HttpGet("at/{at}")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEvents(DateTime at)
        {
            return this.Ok(await _eventsFacade.GetEvents(at));
        }

        /// <summary>
        /// Returns an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to return.</param>
        /// <response code="200">The event.</response>
        /// <response code="404">No such event exists.</response>
        [AllowAnonymous]
        [HttpGet("{eventId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// Returns a list of events that are happening at the moment.
        /// </summary>
        /// <response code="200">The list of current events.</response>
        [AllowAnonymous]
        [HttpGet("current")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentEvents()
        {
            return this.Ok(await _eventsFacade.GetCurrentEvents());
        }

        /// <summary>
        /// Returns a list of the next planned events.
        /// </summary>
        /// <remarks>
        /// Several events may start at the same time. Thus, a list is returned instead of a single event.
        /// </remarks>
        /// <response code="200">The list of the next planned events.</response>
        [AllowAnonymous]
        [HttpGet("next")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNextPlannedEvents()
        {
            return this.Ok(await _eventsFacade.GetNextPlannedEvents());
        }

        /// <summary>
        /// Returns a list of events planned in the specified time range.
        /// </summary>
        /// <response code="200">The list of events.</response>
        /// <response code="400">The specified time range is too long or `<paramref name="to"/>` comes before
        /// `<paramref name="from"/>`.</response>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEvents(
            DateTime? from = null,
            DateTime? to = null)
        {
            try
            {
                var events = await _eventsFacade.GetEvents(from, to);
                return this.Ok(events);
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// Plans a new event.
        /// </summary>
        /// <param name="newEvent">An event to create.</param>
        /// <returns>A planned <see cref="KachnaOnline.Dto.Events.EventDto"/> if the creation succeeded.</returns>
        /// <response code="201">The new event.</response>
        /// <response code="400">Invalid event parameters.</response>
        /// <response code="422">The user does not exist.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
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
                return this.Conflict();
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
        /// Replaces details of an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to update.</param>
        /// <param name="baseEvent">An event model with the new event details.</param>
        /// <response code="204">The event was updated.</response>
        /// <response code="404">The event with the given ID does not exist.</response>
        /// <response code="409">The event from the past cannot be modified.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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
                    detail: "Modification of the event failed.",
                    statusCode: 500);
            }
        }

        /// <summary>
        /// Deletes an event with the given ID.
        /// </summary>
        /// <param name="eventId">ID of the event to delete.</param>
        /// <response code="204">The event was deleted.</response>
        /// <response code="404">The event does not exist.</response>
        /// <response code="409">A past event cannot be removed.</response>
        [HttpDelete("{eventId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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
