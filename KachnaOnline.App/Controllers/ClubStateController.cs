using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.ClubStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("states")]
    [Authorize(Roles = AuthConstants.StatesManager)]
    public class ClubStateController : ControllerBase
    {
        private readonly ClubStateFacade _facade;
        private readonly ILogger<ClubStateController> _logger;

        public ClubStateController(ClubStateFacade facade, ILogger<ClubStateController> logger)
        {
            _facade = facade;
            _logger = logger;
        }

        /// <summary>
        /// Returns details of the current state.
        /// </summary>
        /// <remarks>
        /// If no state is active (it is closed), a Closed-type state is returned.
        /// Its `start` property points to the end of the last state, its `plannedEnd`
        /// property points to the beginning of the next planned state. If no state is planned, it is null.
        /// </remarks>
        /// <response code="200">The current state.</response>
        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<StateDto>> GetCurrent()
        {
            return await _facade.GetCurrent();
        }

        /// <summary>
        /// Returns details of a state with the given ID.
        /// </summary>
        /// <param name="id">The ID of the state to return.</param>
        /// <response code="200">The state.</response>
        /// <response code="404">No such state exists or it is not visible to the current user.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<StateDto>> Get(int id)
        {
            var dto = await _facade.Get(id);
            if (dto is null)
                return this.NotFoundProblem("The specified state plan record does not exist.");

            return dto;
        }

        /// <summary>
        /// Returns an array of details of near states or states starting in the specified time range.
        /// </summary>
        /// <remarks>
        /// The default maximum time range to return states in is 62 days.
        /// If one of `<paramref name="from"/>` or `<paramref name="to"/>` is not provided,
        /// the other will be set so that the maximum time range is used.
        /// If both are not provided, `<paramref name="from"/>` will be set to the first day of the current
        /// month.
        /// </remarks>
        /// <param name="from">The start of the target time range.</param>
        /// <param name="to">The end of the target time range.</param>
        /// <response code="200">An array of states.</response>
        /// <response code="400">The specified time range is too long or `<paramref name="to"/>` comes before
        /// `<paramref name="from"/>`.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<List<StateDto>>> GetNearOrBetween(DateTime? from, DateTime? to)
        {
            var res = await _facade.GetNearOrBetween(from, to);
            if (res is null)
                return this.BadRequestProblem("The specified time range is not valid.");

            return res;
        }

        /// <summary>
        /// Returns details of the next planned state of the given type.
        /// </summary>
        /// <param name="type">The state type.</param>
        /// <response code="200">The state.</response>
        /// <response code="403">The selected state type is not visible to the current user.</response>
        /// <response code="404">No state of the given type is planned.</response>
        [HttpGet("next")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<StateDto>> GetNext(StateType? type)
        {
            if (type == StateType.Private)
            {
                if (!this.User.IsInRole(AuthConstants.StatesManager))
                    return this.ForbiddenProblem("You cannot access private state plan records.");
            }

            var dto = await _facade.GetNext(type);
            if (dto is null)
                return this.NotFoundProblem("No state of the given type is planned.", "No such state");

            return dto;
        }

        /// <summary>
        /// Plans a new state.
        /// </summary>
        /// <remarks>
        /// A state cannot be planned if it begins earlier than an already planned state that it would overlap with.
        ///
        /// However, it can be planned so that it begins in the middle of an already planned state. In that case,
        /// the `plannedEnd` of the previously planned state is set to the newly planned state's beginning.
        ///
        /// Planned start must be set to a date after now and before the state's planned end.
        ///
        /// Planned end must be set to a date after the state's start. If it's set to null, the start of the following
        /// planned state will be used as the planned end (if such state exists).
        /// </remarks>
        /// <param name="newState">A new state model.</param>
        /// <response code="201">Details of the newly created state and, potentially, of a state that
        /// has been shortened by the new state.</response>
        /// <response code="400">Some of the restrictions are violated.</response>
        /// <response code="404">No `plannedEnd` was specified and there is no state planned in the future.</response>
        /// <response code="409">The state would collide with existing states. Details of these is returned.</response>
        [HttpPost]
        [ProducesResponseType(typeof(StatePlanningSuccessResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatePlanningConflictResultDto), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> PlanNew(StatePlanningDto newState)
        {
            try
            {
                var result = await _facade.PlanNew(newState);
                if (result.SuccessResultDto != null)
                {
                    return this.CreatedAtAction("Get", new { id = result.SuccessResultDto.NewState.Id },
                        result.SuccessResultDto);
                }

                return this.Conflict(result.ConflictResultDto);
            }
            catch (StateNotFoundException)
            {
                return this.NotFoundProblem(
                    "No planned end date was specified and there is no state planned in the future.",
                    "Following state not found");
            }
            catch (ArgumentException e)
            {
                // TODO: Don't use the message from the exception?
                return this.BadRequestProblem(e.Message);
            }
        }

        /// <summary>
        /// Changes details of the current state.
        /// </summary>
        /// <remarks>
        /// A state cannot be changed in a way that would cause another state's <b>start</b> to be changed.
        ///
        /// For the current state, the planned end and both notes can be changed. Its start cannot be changed.
        ///
        /// Planned end must be set to a date after the state's start.
        ///
        /// Administrators (only) can also change `madeById`.
        /// </remarks>
        /// <param name="data">A modification model.</param>
        /// <response code="200">Details of the modified state and, potentially, of a state that
        /// has been shortened because of a change of the modified state's start date.</response>
        /// <response code="400">Some of the restrictions are violated.</response>
        /// <response code="403">A non-administrator user attempted to change `madeById`.</response>
        /// <response code="404">No state is currently active.</response>
        /// <response code="409">The state would collide with existing states. Details of these are returned.</response>
        /// <response code="422">The user does not exist.</response>
        [HttpPatch("current")]
        [ProducesResponseType(typeof(StatePlanningSuccessResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatePlanningConflictResultDto), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> ModifyCurrent(StateModificationDto data)
        {
            return await this.ModifyCurrentOrSpecified(null, data);
        }

        /// <summary>
        /// Changes details of a state with the given ID. Past states cannot be modified.
        /// </summary>
        /// <remarks>
        /// A state cannot be changed in a way that would cause another state's <b>start</b> to be changed.
        ///
        /// For the current state and planned states, the planned end and both notes can be changed.
        ///
        /// Planned end must be set to a date after the state's start.
        ///
        /// The planned start can only be changed for states that haven't started yet.
        /// Planned start must be set to a date after now and before the state's planned end.
        ///
        /// For states that already ended, only the internal note can be changed.
        ///
        /// Administrators (only) can also change `madeById`.
        /// </remarks>
        /// <param name="id">The ID of the state to change.</param>
        /// <param name="data">A modification model.</param>
        /// <response code="200">Details of the modified state and, potentially, of a state that
        /// has been shortened because of a change of the modified state's start date.</response>
        /// <response code="400">Some of the restrictions are violated.</response>
        /// <response code="403">A non-administrator user attempted to change `madeById`.</response>
        /// <response code="404">The state does not exist.</response>
        /// <response code="409">The state would collide with existing states. Details of these are returned.</response>
        /// <response code="422">The user does not exist.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(StatePlanningSuccessResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatePlanningConflictResultDto), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Modify(int id, StateModificationDto data)
        {
            return await this.ModifyCurrentOrSpecified(id, data);
        }

        /// <summary>
        /// Deletes a state with the given ID that hasn't started yet.
        /// </summary>
        /// <remarks>
        /// A state that has started and hasn't ended, that is, the current state, may be closed using DELETE
        /// `/states/current`. An ended state cannot be deleted.
        /// </remarks>
        /// <param name="id">The ID of the state to delete.</param>
        /// <response code="204">The state was deleted.</response>
        /// <response code="404">The state does not exist.</response>
        /// <response code="409">The state has already started.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _facade.Delete(id);
                return this.NoContent();
            }
            catch (StateReadOnlyException)
            {
                return this.ConflictProblem("The state has already started or ended and cannot be modified.",
                    "Cannot modify past state");
            }
            catch (StateNotFoundException)
            {
                return this.NotFoundProblem("The specified state plan record does not exist.");
            }
        }

        /// <summary>
        /// Closes the current state.
        /// </summary>
        /// <returns></returns>
        /// <response code="204">The current state was closed.</response>
        /// <response code="404">The club is already closed, no state is currently active.</response>
        [HttpDelete("current")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloseCurrent()
        {
            try
            {
                await _facade.CloseCurrent();
                return this.NoContent();
            }
            catch (StateNotFoundException)
            {
                return this.NotFoundProblem("The specified state plan record does not exist.");
            }
        }

        private async Task<ActionResult> ModifyCurrentOrSpecified(int? id, StateModificationDto data)
        {
            try
            {
                var result = id.HasValue ? await _facade.Modify(id.Value, data) : await _facade.ModifyCurrent(data);
                if (result.SuccessResultDto != null)
                    return this.Ok(result.SuccessResultDto);

                return this.Conflict(result.ConflictResultDto);
            }
            catch (StateNotFoundException)
            {
                return this.NotFoundProblem(id.HasValue
                    ? "The specified state plan record does not exist."
                    : "No state is active at the moment.");
            }
            catch (InvalidOperationException e)
            {
                // TODO: Don't use the message from the exception?
                return this.BadRequestProblem(e.Message);
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntityProblem("The specified user does not exist.");
            }
            catch (UserUnprivilegedException)
            {
                return this.ForbiddenProblem("Only administrators may change a state's made by ID.");
            }
        }

        /// <summary>
        /// Unlinks the specified linked state from all events.
        /// </summary>
        /// <param name="id">The ID of the state.</param>
        /// <response code="204">The state was unlinked from events.</response>
        /// <response code="404">No such state exists.</response>
        /// <response code="409">The state has already started and cannot be modified.</response>
        [HttpDelete("{id}/linkedEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UnlinkStateFromEvent(int id)
        {
            try
            {
                await _facade.UnlinkStateFromEvent(id);
                return this.NoContent();
            }
            catch (StateReadOnlyException)
            {
                return this.ConflictProblem(
                    "The specified state cannot be modified because it has already started.");
            }
            catch (StateNotAssociatedToEventException)
            {
                return this.NoContent();
            }
            catch (StateNotFoundException)
            {
                return this.NotFoundProblem("The specified state does not exist.");
            }
        }

        /// <summary>
        /// Returns details of the current state in the legacy (IsKachnaOpen) format.
        /// </summary>
        [HttpGet("current/legacy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LegacyStateDto>> GetCurrentLegacy()
        {
            return await _facade.GetCurrentLegacy();
        }
    }
}
