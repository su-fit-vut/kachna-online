// UserController.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Roles;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        /// Returns the list of all users.
        /// </summary>
        /// <returns>A list of <see cref="UserDto"/></returns>
        /// <response code="200">The list of users.</response>
        [Authorize(Roles=RoleConstants.Admin)]
        [ProducesResponseType(200)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return new ActionResult<IEnumerable<UserDto>>(await _facade.GetUsers());
        }

        /// <summary>
        /// Returns a user with the given ID.
        /// </summary>
        /// <param name="id">ID of the user to return.</param>
        /// <returns><see cref="UserDto"/> with the given <paramref name="id"/>.</returns>
        /// <response code="200">The user.</response>
        /// <response code="404">No such user exists.</response>
        [Authorize(Roles=RoleConstants.Admin)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("id")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                return new ActionResult<UserDto>(await _facade.GetUser(id));
            }
            catch (UserNotFoundException)
            {
                return this.NotFound();
            }
        }
        
        /// <summary>
        /// Returns information about the currently authenticated user.
        /// </summary>
        /// <returns><see cref="UserDto"/> of the authenticated user.</returns>
        /// <response code="200">The user.</response>
        [ProducesResponseType(200)]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var id = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
            return new ActionResult<UserDto>(await _facade.GetUser(id));
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="id">ID of the user to assign the role to.</param>
        /// <param name="roleId">ID of the role to assign.</param>
        /// <response code="204">The role has been assigned.</response>
        /// <response code="404">No user with ID <paramref name="id"/> exists.</response>
        /// <response code="422">No role with ID <paramref name="roleId"/> exists.</response>
        [Authorize(Roles=RoleConstants.Admin)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [HttpPost("{id}/roles")]
        public async Task<ActionResult> AssignRole(int id, int roleId)
        {
            try
            {
                var assignedBy = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
                await _facade.AssignRole(assignedBy, id, roleId);
                return this.NoContent();
            }
            catch (UserNotFoundException)
            {
                return this.NotFound();
            }
            catch (RoleNotFoundException)
            {
                return this.UnprocessableEntity();
            }
            catch (RoleManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Revokes a role from a user.
        /// </summary>
        /// <param name="id">ID of the user to revoke the role from.</param>
        /// <param name="roleId">ID of the role to revoke from the user.</param>
        /// <response code="204">The role has been revoked.</response>
        /// <response code="404">No user with <paramref name="id"/> exists or the role with ID
        /// <paramref name="roleId"/> was not assigned.</response>
        [Authorize(Roles=RoleConstants.Admin)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}/roles/{roleId}")]
        public async Task<ActionResult> RevokeRole(int id, int roleId)
        {
            try
            {
                await _facade.RevokeRole(id, roleId);
                return this.NoContent();
            }
            catch (RoleNotFoundException)
            {
                return this.NotFound();
            }
            catch (RoleManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }
    }
}
