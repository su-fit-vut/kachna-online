using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.ClubStates;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.ClubStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("states/repeating")]
    [Authorize(Roles = RoleConstants.StatesManager)]
    public class RepeatingStatesController : ControllerBase
    {
        private readonly RepeatingStatesFacade _facade;

        public RepeatingStatesController(RepeatingStatesFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Returns details of all repeating states that are currently active.
        /// </summary>
        /// <remarks>
        /// A repeating state is active if the current date is between its `effectiveFrom` and `effectiveTo` dates.
        /// </remarks>
        /// <response code="200">An array of repeating states.</response>
        [HttpGet("active/now")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RepeatingStateDto>>> Get()
        {
            return await _facade.Get(DateTime.Now);
        }

        /// <summary>
        /// Returns details of all repeating states that were, are or will be active at the given date.
        /// </summary>
        /// <remarks>
        /// A repeating state is active if the given date is between its `effectiveFrom` and `effectiveTo` dates.
        /// </remarks>
        /// <param name="date">The date.</param>
        /// <response code="200">An array of repeating states.</response>
        [HttpGet("active/at")]
        [ProducesResponseType(typeof(List<RepeatingStateManagerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RepeatingStateDto>>> Get([Required] DateTime date)
        {
            return await _facade.Get(date);
        }

        /// <summary>
        /// Returns details of all repeating states, optionally bounded by the given effective from and to dates.
        /// </summary>
        /// <remarks>
        /// <code><![CDATA[effectiveFrom <= from <= to <= effectiveTo]]></code> will always apply to the returned states.
        /// </remarks>
        /// <param name="from">An upper bound for the returned states' `effectiveFrom`.</param>
        /// <param name="to">A lower bound for the returned states' `effectiveTo`.</param>
        /// <response code="200">An array of repeating states.</response>
        /// <response code="400">`<paramref name="to"/>` comes before `<paramref name="from"/>`.</response>
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<RepeatingStateManagerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RepeatingStateDto>>> Get(DateTime? from, DateTime? to)
        {
            if (from > to)
            {
                return this.BadRequest();
            }

            return await _facade.Get(from, to);
        }

        /// <summary>
        /// Returns details of all states that are occurrences of the given repeating state.
        /// </summary>
        /// <param name="id">The ID of the repeating state.</param>
        /// <param name="futureOnly">If set, only states that haven't started yet will be returned.</param>
        /// <response code="200">An array of states.</response>
        /// <response code="404">The repeating state does not exist.</response>
        [HttpGet("{id}/occurrences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<StateDto>>> GetLinkedStates(int id, bool futureOnly)
        {
            var response = await _facade.GetLinkedStates(id, futureOnly);
            if (response is null)
            {
                return this.NotFound();
            }

            return response;
        }

        /// <summary>
        /// Plans a repeating state.
        /// </summary>
        /// <remarks>
        /// When a repeating state is planned, new states, its occurrences, are planned too. The planning behaviour
        /// is the same as in `POST /states`. If a collision with an already planned state should occur, the colliding
        /// state occurrence is not planned and information about this is given in the response. However, this endpoint
        /// returns 200 OK even if some collisions occur.
        ///
        /// `effectiveTo` must be set to a date after `effectiveFrom`.
        ///
        /// `timeTo` must represent a time later than `timeFrom`.
        ///
        /// `state` must not be `Closed`.
        /// </remarks>
        /// <param name="data">A new repeating state model.</param>
        /// <response code="200">Details of the newly created repeating state and, potentially, the planning collisions.
        /// </response>
        /// <response code="400">Some of the restrictions are violated.</response>
        [HttpPost]
        [ProducesResponseType(typeof(RepeatingStatePlanningResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Plan(RepeatingStatePlanningDto data)
        {
            try
            {
                var result = await _facade.Plan(data);

                return result.TargetRepeatingState is null
                    ? this.StatusCode(StatusCodes.Status500InternalServerError, result)
                    : this.Ok(result);
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// Changes details of a repeating state with the given ID. Projects the changes into its future occurrences.
        /// </summary>
        /// <remarks>
        /// If `effectiveTo` is changed, new states may be planned or existing planned states may be removed. It cannot
        /// be changed to a date in the past. The planning behaviour is the same as in `POST /states/repeating`
        ///
        /// Repeating states of which the `effectiveTo` date has already passed (or is today) cannot be modified at all.
        /// 
        /// Administrators (only) can also change `madeById`.
        ///
        /// `state` must not be `Closed`.
        /// </remarks>
        /// <param name="id">The ID of the repeating state.</param>
        /// <param name="data">A repeating state modification model.</param>
        /// <response code="200">Details of the modified repeating state and, potentially, the planning collisions.</response>
        /// <response code="400">Some of the restrictions are violated.</response>
        /// <response code="403">A non-administrator user attempted to change `madeById`.</response>
        /// <response code="404">The repeating state does not exist.</response>
        /// <response code="409">The repeating state has already ended.</response>
        /// <response code="422">The user does not exist.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RepeatingStatePlanningResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Modify(int id, RepeatingStateModificationDto data)
        {
            RepeatingStatePlanningResultDto result;

            try
            {
                result = await _facade.Modify(id, data);
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (RepeatingStateNotFoundException)
            {
                return this.NotFound("The specified repeating state does not exist.");
            }
            catch (RepeatingStateReadOnlyException)
            {
                return this.Conflict("The specified repeating state has already ended.");
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntity("The specified user does not exist.");
            }
            catch (UserUnprivilegedException)
            {
                return this.Forbid();
            }

            return result.TargetRepeatingState is null
                ? this.StatusCode(StatusCodes.Status500InternalServerError, result)
                : this.Ok(result);
        }

        /// <summary>
        /// Deletes an inactive repeating state or deletes all active repeating state's future occurrences.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// If the repeating state isn't active yet (its `effectiveFrom` is in future), the repeating state entry
        /// and all its planned occurrences are deleted.
        ///
        /// If it is active, only the future occurrences (that haven't
        /// started yet) are deleted and the repeating state's `effectiveTo` is set to today, making it ended and
        /// immutable. This operation is the same as if it was modified this way using `PATCH /states/repeating/{id}`.
        /// </remarks>
        /// <response code="200">An active repeating state's occurrences were deleted; its details are returned.</response>
        /// <response code="204">The repeating state was not active and it was deleted.</response>
        /// <response code="404">The repeating state does not exist.</response>
        /// <response code="409">The repeating state has already ended.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(RepeatingStateManagerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _facade.Delete(id);

                if (result == null)
                {
                    return this.NoContent();
                }

                return this.Ok(result);
            }
            catch (RepeatingStateNotFoundException)
            {
                return this.NotFound("The specified repeating state does not exist.");
            }
            catch (RepeatingStateReadOnlyException)
            {
                return this.Conflict("The specified repeating state has already ended.");
            }
        }
    }
}
