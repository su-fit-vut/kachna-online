using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Cleanings;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.ClubStates;
using KachnaOnline.Dto.Cleanings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static KachnaOnline.App.Controllers.UserController;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("cleanings")]
    [Authorize(Roles = AuthConstants.CleaningsManager)]
    public class CleaningsController : ControllerBase
    {
        private readonly CleaningsFacade _cleaningsFacade;

        // private CleaningViewDto linkUsernamesFromId(CleaningDto cleaning) {
        //     var linkedCleaning = new CleaningViewDto
        //     {
        //         Name = cleaning.Name,
        //         Place = cleaning.Place,
        //         CleaningInstructions = cleaning.CleaningInstructions,
        //         AssignedUsersIds = cleaning.AssignedUsersIds,
        //         IdealParticipantsCount = cleaning.IdealParticipantsCount,
        //         Finished = cleaning.Finished,
        //         From = cleaning.From,
        //         To = cleaning.To,
        //         LinkedPlannedStateIds = cleaning.LinkedPlannedStateIds,
        //         Id = cleaning.Id,
        //         IdToUsername = []
        //     };

        //     foreach (var userId in cleaning.AssignedUsersIds)
        //     {
        //         var userName = 
        //         linkedCleaning.IdToUsername = { userId, };
        //     }

        //     return linkedCleaning;
        // }

        public CleaningsController(CleaningsFacade cleaningsFacade)
        {
            _cleaningsFacade = cleaningsFacade;
        }

        /// <summary>
        /// Returns a list of cleanings being held at the given date and time.
        /// </summary>
        /// <param name="at">The date and time.</param>
        /// <response code="200">The list of cleanings.</response>
        [AllowAnonymous]
        [HttpGet("at/{at}")]
        [ProducesResponseType(typeof(IEnumerable<CleaningDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCleanings(DateTime at)
        {
            return this.Ok(await _cleaningsFacade.GetCleanings(at));
        }

        /// <summary>
        /// Returns an cleaning with the given ID.
        /// </summary>
        /// <param name="id">ID of the cleaning to return.</param>
        /// <response code="200">The cleaning.</response>
        /// <response code="404">No such cleaning exists.</response>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CleaningDto>> GetCleaning(int id)
        {
            try
            {
                return await _cleaningsFacade.GetCleaning(id);
            }
            catch (CleaningNotFoundException)
            {
                return this.NotFoundProblem("The specified cleaning does not exist.");
            }
        }

        /// <summary>
        /// Returns a list of cleanings that are happening at the moment.
        /// </summary>
        /// <response code="200">The list of current cleanings.</response>
        [AllowAnonymous]
        [HttpGet("current")]
        [ProducesResponseType(typeof(IEnumerable<CleaningDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentCleanings()
        {
            return this.Ok(await _cleaningsFacade.GetCurrentCleanings());
        }

        /// <summary>
        /// Returns a list of the next planned cleanings.
        /// </summary>
        /// <remarks>
        /// Several cleanings may start at the same time. Thus, a list is returned instead of a single cleaning.
        /// </remarks>
        /// <response code="200">The list of the next planned cleanings.</response>
        [AllowAnonymous]
        [HttpGet("next")]
        [ProducesResponseType(typeof(IEnumerable<CleaningDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNextPlannedCleanings()
        {
            return this.Ok(await _cleaningsFacade.GetNextPlannedCleanings());
        }

        /// <summary>
        /// Returns a list of cleanings planned in the specified time range.
        /// </summary>
        /// <response code="200">The list of cleanings.</response>
        /// <response code="400">The specified time range is too long or `<paramref name="to"/>` comes before
        /// `<paramref name="from"/>`.</response>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CleaningDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCleanings(
            DateTime? from = null,
            DateTime? to = null)
        {
            try
            {
                var cleanings = await _cleaningsFacade.GetCleanings(from, to);
                return this.Ok(cleanings);
            }
            catch (ArgumentException)
            {
                return this.BadRequestProblem("The specified time range is not valid.");
            }
        }

        /// <summary>
        /// Plans a new cleaning.
        /// </summary>
        /// <param name="newCleaning">An cleaning to create.</param>
        /// <response code="201">The new cleaning.</response>
        /// <response code="400">Invalid cleaning parameters.</response>
        /// <response code="422">The user does not exist.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ManagerCleaningDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ManagerCleaningDto>> PlanCleaning(BaseCleaningDto newCleaning)
        {
            try
            {
                var createdCleaning = await _cleaningsFacade.PlanCleaning(newCleaning);
                return this.CreatedAtAction("GetCleaning", new { id = createdCleaning.Id }, createdCleaning);
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntityProblem("The specified user does not exist.");
            }
            catch (ArgumentException)
            {
                return this.BadRequestProblem("Invalid cleaning parameters.");
            }
        }

        /// <summary>
        /// Replaces details of an cleaning with the given ID.
        /// </summary>
        /// <param name="id">ID of the cleaning to update.</param>
        /// <param name="modifiedCleaning">An cleaning model with the new cleaning details.</param>
        /// <response code="204">The cleaning was updated.</response>
        /// <response code="404">No such cleaning exists.</response>
        /// <response code="409">The cleaning has already ended and cannot be modified.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyCleaning(int id, BaseCleaningDto modifiedCleaning)
        {
            try
            {
                await _cleaningsFacade.ModifyCleaning(id, modifiedCleaning);
                return this.NoContent();
            }
            catch (NotACleaningsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (ArgumentNullException)
            {
                // Shouldn't happen.
                return this.BadRequest();
            }
            catch (CleaningNotFoundException)
            {
                return this.NotFoundProblem("The specified cleaning does not exist.");
            }
            catch (CleaningReadOnlyException)
            {
                return this.ConflictProblem("The specified cleaning has already passed and cannot be modified.");
            }
        }

        /// <summary>
        /// Deletes an cleaning with the given ID.
        /// </summary>
        /// <param name="id">ID of the cleaning to delete.</param>
        /// <response code="200">The cleaning was deleted.</response>
        /// <response code="404">No such cleaning exists.</response>
        /// <response code="409">The cleaning has already ended and cannot be modified.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(IEnumerable<StateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveCleaning(int id)
        {
            try
            {
                await _cleaningsFacade.RemoveCleaning(id);
                return this.Ok();
            }
            catch (NotACleaningsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (CleaningReadOnlyException)
            {
                return this.ConflictProblem("The specified cleaning has already passed and cannot be deleted.");
            }
            catch (CleaningNotFoundException)
            {
                return this.NotFoundProblem("The specified cleaning does not exist.");
            }
        }

        // <summary>
        /// Joins a cleaning with the given ID.
        /// </summary>
        /// <param name="id">ID of the cleaning to join.</param>
        /// <response code="200">The cleaning was joined.</response>
        /// <response code="404">No such cleaning exists.</response>
        /// <response code="409">The cleaning has already ended and cannot be joined.</response>
        [HttpPost("join/{id}")]
        [ProducesResponseType(typeof(IEnumerable<StateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> JoinCleaning(int id)
        {
            try
            {
                await _cleaningsFacade.JoinCleaning(id);
                return this.Ok();
            }
            catch (NotACleaningsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (CleaningReadOnlyException)
            {
                return this.ConflictProblem("The specified cleaning has already passed and cannot be joined.");
            }
            catch (CleaningNotFoundException)
            {
                return this.NotFoundProblem("The specified cleaning does not exist.");
            }
        }

        // <summary>
        /// Leaves a cleaning with the given ID.
        /// </summary>
        /// <param name="id">ID of the cleaning to leave.</param>
        /// <response code="200">The cleaning was left.</response>
        /// <response code="404">No such cleaning exists.</response>
        /// <response code="409">The cleaning has already ended and cannot be left.</response>
        [HttpPost("leave/{id}")]
        [ProducesResponseType(typeof(IEnumerable<StateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> LeaveCleaning(int id)
        {
            try
            {
                await _cleaningsFacade.LeaveCleaning(id);
                return this.Ok();
            }
            catch (NotACleaningsManagerException)
            {
                // Shouldn't happen.
                return this.ForbiddenProblem();
            }
            catch (CleaningReadOnlyException)
            {
                return this.ConflictProblem("The specified cleaning has already passed and cannot be left.");
            }
            catch (CleaningNotFoundException)
            {
                return this.NotFoundProblem("The specified cleaning does not exist.");
            }
        }
    }
}
