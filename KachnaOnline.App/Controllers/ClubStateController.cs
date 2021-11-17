// ClubStateController.cs
// Author: Ondřej Ondryáš

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
using Microsoft.Extensions.Logging;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("states")]
    [Authorize(Roles = RoleConstants.StatesManager)]
    public class ClubStateController : ControllerBase
    {
        private readonly ClubStateFacade _facade;
        private readonly ILogger<ClubStateController> _logger;

        public ClubStateController(ClubStateFacade facade, ILogger<ClubStateController> logger)
        {
            _facade = facade;
            _logger = logger;
        }

        [HttpGet("current")]
        [AllowAnonymous]
        public async Task<ActionResult<StateDto>> GetCurrent()
        {
            return await _facade.GetCurrent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<StateDto>> Get(int id)
        {
            var dto = await _facade.Get(id);
            if (dto is null)
                return this.NotFound();

            return dto;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<List<StateDto>>> GetNearOrBetween(DateTime? from, DateTime? to)
        {
            var res = await _facade.GetNearOrBetween(from, to);
            if (res is null)
                return this.BadRequest();

            return res;
        }

        [HttpGet("next")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<StateDto>> GetNext([Required] StateType type)
        {
            if (type == StateType.Private)
            {
                if (!this.User.IsInRole(RoleConstants.StatesManager))
                {
                    return this.Forbid();
                }
            }

            var dto = await _facade.GetNext(type);
            if (dto is null)
                return this.NotFound();

            return dto;
        }

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
                return this.NotFound(
                    $"{nameof(newState.PlannedEnd)} must be specified because no state is planned after this one.");
            }
            catch (ArgumentException e)
            {
                return this.BadRequest(e.Message);
            }
        }

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
                return this.Conflict("The state has already started and cannot be deleted.");
            }
            catch (StateNotFoundException)
            {
                return this.NotFound();
            }
        }

        [HttpDelete("current")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CloseCurrent()
        {
            try
            {
                await _facade.CloseCurrent();
                return this.NoContent();
            }
            catch (StateNotFoundException)
            {
                return this.NotFound();
            }
            catch (InvalidOperationException)
            {
                return this.Conflict();
            }
        }

        private async Task<ActionResult> ModifyCurrentOrSpecified(int? id, StateModificationDto data)
        {
            try
            {
                var result = id.HasValue ? await _facade.Modify(id.Value, data) : await _facade.ModifyCurrent(data);
                if (result.SuccessResultDto != null)
                {
                    return this.Ok(result.SuccessResultDto);
                }

                return this.Conflict(result.ConflictResultDto);
            }
            catch (StateNotFoundException)
            {
                return this.NotFound("No state is active at the moment.");
            }
            catch (InvalidOperationException e)
            {
                return this.BadRequest(e.Message);
            }
            catch (UserNotFoundException)
            {
                return this.NotFound("The specified user doesn't exist.");
            }
            catch (UserUnprivilegedException)
            {
                return this.Forbid();
            }
        }
    }
}
