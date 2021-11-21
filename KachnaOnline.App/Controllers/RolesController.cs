// RolesController.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions.Roles;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("roles")]
    [Authorize(Roles = RoleConstants.Admin)]
    public class RolesController : ControllerBase
    {
        private readonly RolesFacade _facade;

        public RolesController(RolesFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Returns the list of all roles.
        /// </summary>
        /// <returns>A list of <see cref="RoleDto"/>.</returns>
        /// <response code="200">The list of roles.</response>
        [ProducesResponseType(200)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var dto = await _facade.GetRoles();
            return new ActionResult<IEnumerable<RoleDto>>(dto);
        }

        /// <summary>
        /// Returns a role with the given ID.
        /// </summary>
        /// <param name="id">ID of the role to return.</param>
        /// <returns>A <see cref="RoleDto"/> with ID <paramref name="id"/>.</returns>
        /// <response code="200">The role.</response>
        /// <response code="404">No such role exists.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            try
            {
                return new ActionResult<RoleDto>(await _facade.GetRole(id));
            }
            catch (RoleNotFoundException)
            {
                return this.NotFound();
            }
        }
    }
}
