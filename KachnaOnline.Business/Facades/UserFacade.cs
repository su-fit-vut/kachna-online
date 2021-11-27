// UserFacade.cs
// Author: František Nečas, Ondřej Ondryáš

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

        private async Task<UserDto> MapUser(User user)
        {
            var userDto = _mapper.Map<UserDetailsDto>(user);

            var roleDetails = (await _userService.GetUserRoleDetails(user.Id)).ToList();

            userDto.ManuallyAssignedRoles =
                _mapper.Map<UserRoleAssignmentDetailsDto[]>(roleDetails.Where(r => r.AssignedManually));

            userDto.ActiveRoles = roleDetails
                .Where(r => !r.ManuallyDisabled)
                .Select(r => r.Role)
                .ToArray();

            return userDto;
        }

        /// <summary>
        /// Returns a list of all users with their roles.
        /// </summary>
        /// <returns>An enumerable of all <see cref="UserDto"/>.</returns>
        public async Task<List<UserDto>> GetUsers()
        {
            var users = new List<UserDto>();
            foreach (var user in await _userService.GetUsers())
            {
                users.Add(await this.MapUser(user));
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

            return await this.MapUser(user);
        }

        public async Task<List<string>> GetRoles()
        {
            return (await _userService.GetRoles()).ToList();
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="userId">The ID of the user to assign the role to.</param>
        /// <param name="role">The name of the role to assign.</param>
        /// <param name="assignedByUserId">The ID of the user making the assignment.</param>
        /// <exception cref="UserNotFoundException">When the user does not exist.</exception>
        /// <exception cref="RoleNotFoundException">When the role does not exist.</exception>
        /// <exception cref="RoleManipulationFailedException">When the assignment failed.</exception>
        public async Task AssignRole(int userId, string role, int assignedByUserId)
        {
            await _userService.AssignRole(userId, role, assignedByUserId);
        }

        /// <summary>
        /// Revokes a role from a user.
        /// </summary>
        /// <param name="userId">The ID of the user to revoke the role from.</param>
        /// <param name="role">The ID of the role to revoke.</param>
        /// <param name="revokedByUserId">The ID of the user revoking the role.</param>
        /// <exception cref="RoleNotFoundException">When the assignment does not exist.</exception>
        /// <exception cref="RoleManipulationFailedException">When the revocation failed.</exception>
        public async Task RevokeRole(int userId, string role, int revokedByUserId)
        {
            await _userService.RevokeRole(userId, role, revokedByUserId);
        }

        /// <summary>
        /// Resets a role assignment to the default state (KIS-mapped).
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="resetByUserId"></param>
        public async Task ResetRole(int userId, string role, int resetByUserId)
        {
            await _userService.ResetRole(userId, role, resetByUserId);
        }

        public async Task SetDiscordId(int userId, ulong? discordId)
        {
            var user = await _userService.GetUser(userId);
            if (user is null)
                throw new UserNotFoundException(userId);

            user.DiscordId = discordId;
            await _userService.SaveUser(user);
        }

        public async Task SetNickname(int userId, string nickname)
        {
            var user = await _userService.GetUser(userId);
            if (user is null)
                throw new UserNotFoundException(userId);

            user.Nickname = nickname;
            await _userService.SaveUser(user);
        }
    }
}
