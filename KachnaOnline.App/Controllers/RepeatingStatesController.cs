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

        [HttpGet("effective/now")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RepeatingStateDto>>> Get()
        {
            return await _facade.Get(DateTime.Now);
        }

        [HttpGet("effective/between")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RepeatingStateDto>>> Get(DateTime? from, DateTime? to)
        {
            if (from > to)
            {
                return this.BadRequest();
            }

            return await _facade.Get(from, to);
        }

        [HttpGet("effective/at")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RepeatingStateDto>>> Get([Required] DateTime date)
        {
            return await _facade.Get(date);
        }

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

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RepeatingStatePlanningResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                return this.UnprocessableEntity("The specified repeating state has already ended.");
            }
            catch (UserNotFoundException)
            {
                return this.NotFound("The specified user does not exist.");
            }
            catch (UserUnprivilegedException)
            {
                return this.Forbid();
            }

            return result.TargetRepeatingState is null
                ? this.StatusCode(StatusCodes.Status500InternalServerError, result)
                : this.Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(RepeatingStateManagerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                return this.UnprocessableEntity("The specified repeating state has already ended.");
            }
        }
    }
}
