// EventsController.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Exceptions.Events;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.ClubStates;
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
        /// <param name="id">ID of the event to return.</param>
        /// <param name="withLinkedStates">Whether to return linked states as well.</param>
        /// <response code="200">The event.</response>
        /// <response code="404">No such event exists.</response>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventDto>> GetEvent(int id, bool withLinkedStates = false)
        {
            try
            {
                return await _eventsFacade.GetEvent(id, withLinkedStates);
            }
            catch (EventNotFoundException)
            {
                return this.NotFoundProblem("The specified event does not exist.");
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
                return this.BadRequestProblem("The specified time range is not valid.");
            }
        }

        /// <summary>
        /// Plans a new event.
        /// </summary>
        /// <param name="newEvent">An event to create.</param>
        /// <response code="201">The new event.</response>
        /// <response code="400">Invalid event parameters.</response>
        /// <response code="422">The user does not exist.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ManagerEventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ManagerEventDto>> PlanEvent(BaseEventDto newEvent)
        {
            try
            {
                var createdEvent = await _eventsFacade.PlanEvent(newEvent);
                return this.CreatedAtAction("GetEvent", new { id = createdEvent.Id }, createdEvent);
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntityProblem("The specified user does not exist.");
            }
            catch (ArgumentException)
            {
                return this.BadRequestProblem("Invalid event parameters.");
            }
        }

        /// <summary>
        /// Returns a list of conflicting planned states for an event with the given ID.
        /// </summary>
        /// <param name="id">ID of the event to get the conflicting planned states for.</param>
        /// <response code="200">The list of conflicting states.</response>
        /// <response code="404">No such event exists.</response>
        [HttpGet("{id}/conflictingStates")]
        [ProducesResponseType(typeof(IEnumerable<StateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConflictingPlannedStatesForEvent(int id)
        {
            try
            {
                return this.Ok(await _eventsFacade.GetConflictingStatesForEvent(id));
            }
            catch (EventNotFoundException)
            {
                return this.NotFoundProblem("The specified event does not exist.");
            }
        }

        /// <summary>
        /// Returns states linked to an event with the given ID.
        /// </summary>
        /// <param name="id">An ID of the event to link planned states to.</param>
        /// <response code="200">The planned states were linked to the event.</response>
        /// <response code="404">No such event exists.</response>
        [AllowAnonymous]
        [HttpGet("{id}/linkedStates")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLinkedStatesForEvent(int id)
        {
            try
            {
                return this.Ok(await _eventsFacade.GetLinkedStatesForEvent(id));
            }
            catch (EventNotFoundException)
            {
                return this.NotFoundProblem("The specified event does not exist.");
            }
        }

        /// <summary>
        /// Links states to an event with the given ID.
        /// </summary>
        /// <param name="id">An ID of the event to link planned states to.</param>
        /// <param name="plannedStatesToLinkDto">A list of planned state IDs to link to the event specified by <paramref name="id"/>.</param>
        /// <response code="204">The planned states were linked to the event.</response>
        /// <response code="404">No such event exists.</response>
        /// <response code="409">The event has already ended and cannot be modified.</response>
        [HttpPost("{id}/linkedStates")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> LinkPlannedStatesToEvent(int id, PlannedStatesToLinkDto plannedStatesToLinkDto)
        {
            try
            {
                await _eventsFacade.LinkPlannedStatesToEvent(id, plannedStatesToLinkDto);
                return this.NoContent();
            }
            catch (EventNotFoundException)
            {
                return this.NotFoundProblem("The specified event does not exist.");
            }
            catch (StateNotFoundException)
            {
                return this.NotFoundProblem("The specified state does not exist.");
            }
            catch (NotAnEventsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (EventReadOnlyException)
            {
                return this.ConflictProblem("The specified event has already passed and cannot be updated.");
            }
            catch (StateReadOnlyException)
            {
                return this.ConflictProblem("The specified state has already started or ended and cannot be updated.");
            }
            catch (LinkingStateToEventException)
            {
                return this.ConflictProblem("Linking specified planned states to event is not possible.");
            }
        }

        /// <summary>
        /// Sets the linked states of an event with the given ID.
        /// </summary>
        /// <param name="id">The ID of the event to set the linked planned states for.</param>
        /// <param name="plannedStatesToLink">A list of planned state IDs to link to the event <paramref name="id"/>.</param>
        /// <response code="204">The planned states were linked to the event.</response>
        /// <response code="404">No such event exists.</response>
        /// <response code="409">The event has already ended and cannot be modified.</response>
        [HttpPut("{id}/linkedStates")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SetLinkedPlannedStatesForEvent(int id,
            PlannedStatesToLinkDto plannedStatesToLink)
        {
            try
            {
                await _eventsFacade.SetLinkedPlannedStatesForEvent(id, plannedStatesToLink);
                return this.NoContent();
            }
            catch (EventNotFoundException)
            {
                return this.NotFoundProblem("The specified event does not exist.");
            }
            catch (StateNotFoundException)
            {
                return this.NotFoundProblem("The specified state does not exist.");
            }
            catch (LinkingStateToEventException)
            {
                return this.ConflictProblem("Linking specified planned states to event is not possible.");
            }
            catch (NotAnEventsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (EventReadOnlyException)
            {
                return this.ConflictProblem("The specified event has already passed and cannot be updated.");
            }
            catch (StateReadOnlyException)
            {
                return this.ConflictProblem("The specified state has already started or ended and cannot be updated.");
            }
        }

        /// <summary>
        /// Unlinks all currently linked states from an event with the given ID.
        /// </summary>
        /// <param name="id">Event to unlink linked planned states from.</param>
        /// <response code="204">The planned states were linked to the event.</response>
        /// <response code="404">No such event exists.</response>
        /// <response code="409">The event has already ended and cannot be modified.</response>
        [HttpDelete("{id}/linkedStates")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ClearLinkedPlannedStatesForEvent(int id)
        {
            try
            {
                await _eventsFacade.ClearLinkedPlannedStatesForEvent(id);
                return this.NoContent();
            }
            catch (EventNotFoundException)
            {
                return this.NotFoundProblem("The specified event does not exist.");
            }
            catch (NotAnEventsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (EventReadOnlyException)
            {
                return this.ConflictProblem("The specified event has already passed and cannot be updated.");
            }
            catch (StateReadOnlyException)
            {
                return this.ConflictProblem("The specified state has already started or ended and cannot be updated.");
            }
        }

        /// <summary>
        /// Replaces details of an event with the given ID.
        /// </summary>
        /// <param name="id">ID of the event to update.</param>
        /// <param name="modifiedEvent">An event model with the new event details.</param>
        /// <response code="204">The event was updated.</response>
        /// <response code="404">No such event exists.</response>
        /// <response code="409">The event has already ended and cannot be modified.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyEvent(int id, BaseEventDto modifiedEvent)
        {
            try
            {
                await _eventsFacade.ModifyEvent(id, modifiedEvent);
                return this.NoContent();
            }
            catch (NotAnEventsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (ArgumentNullException)
            {
                // Shouldn't happen.
                return this.BadRequest();
            }
            catch (EventNotFoundException)
            {
                return this.NotFoundProblem("The specified event does not exist.");
            }
            catch (EventReadOnlyException)
            {
                return this.ConflictProblem("The specified event has already passed and cannot be modified.");
            }
            // TODO: change the underlying logic to throw a more specific exception
            catch (EventManipulationFailedException)
            {
                return this.ConflictProblem("A state outside the new event's time is linked to the event.");
            }
        }

        /// <summary>
        /// Deletes an event with the given ID.
        /// </summary>
        /// <param name="id">ID of the event to delete.</param>
        /// <response code="200">The event was deleted. The list of linked states at the time of deletion.</response>
        /// <response code="404">No such event exists.</response>
        /// <response code="409">The event has already ended and cannot be modified.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(IEnumerable<StateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveEvent(int id)
        {
            try
            {
                return this.Ok(await _eventsFacade.RemoveEvent(id));
            }
            catch (NotAnEventsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (EventReadOnlyException)
            {
                return this.ConflictProblem("The specified event has already passed and cannot be deleted.");
            }
            catch (EventNotFoundException)
            {
                return this.NotFoundProblem("The specified event does not exist.");
            }
        }
    }
}
