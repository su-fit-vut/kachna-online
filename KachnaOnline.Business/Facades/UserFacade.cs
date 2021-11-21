// UserFacade.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.Users;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.Roles;
using KachnaOnline.Business.Models.Users;

namespace KachnaOnline.Business.Facades
{
    public class UserFacade
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserFacade(IUserService userService, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
        }

        /// <summary>
        /// Returns an enumerable of all users.
        /// </summary>
        /// <returns>An enumerable of all <see cref="UserDto"/>.</returns>
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = new List<UserDto>();
            foreach (var user in await _userService.GetUsers())
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = (await _userService.GetUserRoles(userDto.Id)).ToArray();
                users.Add(userDto);
            }

            return users;
        }

        /// <summary>
        /// Returns a user with the given ID.
        /// </summary>
        /// <param name="id">ID of the user to return.</param>
        /// <returns><see cref="UserDto"/> with the ID <paramref name="id"/>.</returns>
        /// <exception cref="UserNotFoundException">When the user does not exist.</exception>
        public async Task<UserDto> GetUser(int id)
        {
            var user = await _userService.GetUser(id);
            if (user is null)
            {
                throw new UserNotFoundException(id);
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = (await _userService.GetUserRoles(id)).ToArray();
            return userDto;
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="assignedBy">ID of the user making the assignment.</param>
        /// <param name="userId">ID of the user to assign the role to.</param>
        /// <param name="roleId">ID of the role to assign.</param>
        /// <exception cref="UserNotFoundException">When the user does not exist.</exception>
        /// <exception cref="RoleAlreadyAssignedException">When the role has already been assigned.</exception>
        /// <exception cref="RoleNotFoundException">When the role does not exist.</exception>
        /// <exception cref="RoleManipulationFailedException">When the assignment failed.</exception>
        public async Task AssignRole(int assignedBy, int userId, int roleId)
        {
            if (await _userService.GetUser(userId) is null)
            {
                throw new UserNotFoundException(userId);
            }

            // This will throw if the role does not exist
            await _userService.GetRole(roleId);
            var assignment = new UserRole()
                { AssignedByUserId = assignedBy, ForceDisable = false, RoleId = roleId, UserId = userId };
            await _userService.AssignRole(assignment);
        }

        /// <summary>
        /// Revokes a role from a user.
        /// </summary>
        /// <param name="userId">ID of the user to revoke the role from.</param>
        /// <param name="roleId">ID of the role to revoke.</param>
        /// <exception cref="RoleNotFoundException">When the assignment does not exist.</exception>
        /// <exception cref="RoleManipulationFailedException">When the revocation failed.</exception>
        public async Task RevokeRole(int userId, int roleId)
        {
            await _userService.RevokeRole(userId, roleId);
        }
    }
}
