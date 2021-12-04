// UserController.cs
// Author: František Nečas, Ondřej Ondryáš

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Roles;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserFacade _facade;

        public UserController(UserFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Returns a list of all users.
        /// </summary>
        /// <remarks>
        /// This endpoint may be used by administrators and board games managers. Administrators may list all users and
        /// get detailed information about the users (UserDetailsDto). Board games managers must specify a filter that
        /// is at least 3 characters long and they only get basic details of the users (UserDto).
        ///
        /// Whitespaces at the start and the end of the filter are trimmed.
        /// </remarks>
        /// <response code="200">The list of users.</response>
        /// <response code="400">The given filter has an effective value shorter than 3 characters.</response>
        /// <response code="403">This user cannot perform this operation. Required roles: Admin (can fetch all users) or BoardGamesManager (must use a filter).</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthConstants.AdminOrBoardGamesManagerPolicy)]
        public async Task<ActionResult<List<UserDto>>> GetUsers(string filter)
        {
            var isAdmin = this.User.IsInRole(AuthConstants.Admin);
            var isFilterShort = filter?.Length < 3;

            if ((string.IsNullOrEmpty(filter) || isFilterShort) && !isAdmin)
                return this.ForbiddenProblem("Only administrators may fetch the full list of users.");

            if (isFilterShort)
                return this.BadRequestProblem("When specifying a filter, it must be at least 3 characters long.");

            return await _facade.GetUsers(filter, isAdmin);
        }

        /// <summary>
        /// Returns a user with the given ID.
        /// </summary>
        /// <param name="id">The ID of the user to return.</param>
        /// <response code="200">The user.</response>
        /// <response code="404">No such user exists.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = AuthConstants.Admin)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                return await _facade.GetUser(id);
            }
            catch (UserNotFoundException)
            {
                return this.NotFoundProblem("The specified user does not exist.");
            }
        }

        /// <summary>
        /// Returns information about the currently authenticated user.
        /// </summary>
        /// <response code="200">The user.</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var id = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
            return await _facade.GetUser(id);
        }

        /// <summary>
        /// Sets the currently authenticated user's Discord ID.
        /// </summary>
        /// <param name="discordId">The new Discord ID. May be null.</param>
        /// <response code="204">The Discord ID has been set.</response>
        [HttpPut("me/discordID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SetUserDiscordId(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            ulong? discordId)
        {
            var id = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
            await _facade.SetDiscordId(id, discordId);
            return this.NoContent();
        }

        /// <summary>
        /// Sets the currently authenticated user's Discord ID.
        /// </summary>
        /// <param name="nickname">The new nickname. If set to null, the user's nickname will be synchronized with KIS
        /// on the next login.</param>
        /// <response code="204">The nickname has been set.</response>
        [HttpPut("me/nickname")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SetUserNickname(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] [StringLength(128)]
            string nickname)
        {
            var id = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
            await _facade.SetNickname(id, nickname);
            return this.NoContent();
        }

        /// <summary>
        /// Changes the manual role assignment state for the given user and role.
        /// </summary>
        /// <remarks>
        /// After performing this operation, the user will always (`state = true`) or never (`state = false`) obtain the
        /// role when logging in, regardless of their KIS roles.
        /// This manual (*forceful*) assignment can be reversed by calling `DELETE /users/{id}/roles/{roleName}/assignment`.
        /// </remarks>
        /// <param name="id">The ID of the user.</param>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="state">The assignment state. If true, the role will be manually assigned. If false,
        /// the role will be manually revoked.</param>
        /// <response code="204">The role assignment state was changed.</response>
        /// <response code="404">The user does not exist.</response>
        /// <response code="422">The role does not exist.</response>
        [HttpPut("{id}/roles/{roleName}/assignment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = AuthConstants.Admin)]
        public async Task<IActionResult> AssignRole(int id, [StringLength(64)] string roleName,
            [Required] [FromQuery] bool state)
        {
            try
            {
                var assignedBy = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));

                if (state)
                {
                    await _facade.AssignRole(id, roleName, assignedBy);
                }
                else
                {
                    await _facade.RevokeRole(id, roleName, assignedBy);
                }

                return this.NoContent();
            }
            catch (UserNotFoundException)
            {
                return this.NotFoundProblem("The specified user does not exist.");
            }
            catch (RoleNotFoundException)
            {
                return this.UnprocessableEntityProblem("The specified role does not exist.");
            }
        }

        /// <summary>
        /// Resets a role assignment to the default state (KIS-mapped).
        /// </summary>
        /// <remarks>
        /// After performing this operation, the manual user-role assignment will be deleted and it will be
        /// automatically mapped based on the user's KIS roles again.
        ///
        /// If the role or such assignment does not exist, a call to this endpoint will be successful and no operation
        /// will be performed.
        /// </remarks>
        /// <param name="id">The ID of the user.</param>
        /// <param name="roleName">The name of the role.</param>
        /// <response code="204">The role assignment state was reset.</response>
        /// <response code="404">The user does not exist.</response>
        [HttpDelete("{id}/roles/{roleName}/assignment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = AuthConstants.Admin)]
        public async Task<IActionResult> ResetRole(int id, [StringLength(64)] string roleName)
        {
            try
            {
                var assignedBy = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
                await _facade.ResetRole(id, roleName, assignedBy);
                return this.NoContent();
            }
            catch (UserNotFoundException)
            {
                return this.NotFoundProblem("The specified user does not exist.");
            }
        }

        /// <summary>
        /// Returns a list of names of roles in the system.
        /// </summary>
        /// <response code="200">The list of roles.</response>
        [HttpGet("/roles")]
        public async Task<List<string>> GetRoles()
        {
            return await _facade.GetRoles();
        }
    }
}
