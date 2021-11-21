// RolesFacade.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Exceptions.Roles;
using KachnaOnline.Business.Models.Users;
using KachnaOnline.Dto.Roles;

namespace KachnaOnline.Business.Facades
{
    public class RolesFacade
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public RolesFacade(IUserService userService, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
        }

        /// <summary>
        /// Returns all roles present in the system.
        /// </summary>
        /// <returns>An enumerable of all roles.</returns>
        public async Task<IEnumerable<RoleDto>> GetRoles()
        {
            return _mapper.Map<IEnumerable<RoleDto>>(await _userService.GetRoles());
        }

        /// <summary>
        /// Returns a role with the given ID.
        /// </summary>
        /// <param name="id">ID of the role to return.</param>
        /// <returns><see cref="RoleDto"/> with ID <paramref name="id"/>.</returns>
        /// <exception cref="RoleNotFoundException">No such role exists.</exception>
        public async Task<RoleDto> GetRole(int id)
        {
            return _mapper.Map<RoleDto>(await _userService.GetRole(id));
        }
    }
}
