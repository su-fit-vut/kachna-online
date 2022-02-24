using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Models.Users;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.ClubStates;
using KachnaOnline.Dto.Users;

namespace KachnaOnline.Business.Extensions
{
    public static class UserServiceExtensions
    {
        /// <summary>
        /// Returns the user corresponding to the 'sub' (user ID) claim in the <paramref name="principal"/>.
        /// </summary>
        /// <param name="userService">An <see cref="IUserService"/> instance.</param>
        /// <param name="principal">The principal that contains the 'sub' (user ID) claim.</param>
        /// <returns>A <see cref="User"/> object containing the user whose ID matches the 'sub' claim value
        /// if such user exists, or null if it doesn't.</returns>
        public static Task<User> GetUser(this IUserService userService, ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(IdentityConstants.IdClaim);
            return int.TryParse(userId, out var userIdValue)
                ? userService.GetUser(userIdValue)
                : Task.FromResult<User>(null);
        }

        /// <summary>
        /// Returns the names of roles assigned to the user corresponding to the 'sub' (user ID) claim
        /// in the <paramref name="principal"/>.
        /// </summary>
        /// <param name="userService">An <see cref="IUserService"/> instance.</param>
        /// <param name="principal">The principal that contains the 'sub' (user ID) claim.</param>
        /// <returns>An enumerable of names of roles assigned to the user whose ID matches the 'sub' claim value
        /// if such user exists, or null if it doesn't.</returns>
        public static Task<IEnumerable<string>> GetUserRoles(this IUserService userService, ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(IdentityConstants.IdClaim);
            return int.TryParse(userId, out var userIdValue)
                ? userService.GetUserRoles(userIdValue)
                : Task.FromResult<IEnumerable<string>>(null);
        }

        /// <summary>
        /// Returns <see cref="RoleAssignment"/> objects describing the role assignments for the user corresponding
        /// to the 'sub' (user ID) claim in the <paramref name="principal"/>.
        /// </summary>
        /// <param name="userService">An <see cref="IUserService"/> instance.</param>
        /// <param name="principal">The principal that contains the 'sub' (user ID) claim.</param>
        /// <returns>An enumerable of <see cref="RoleAssignment"/> objects describing the role assignments for the user
        /// whose ID matches the 'sub' claim value if such user exists, or null if it doesn't.</returns>
        public static Task<IEnumerable<RoleAssignment>> GetUserRoleDetails(this IUserService userService,
            ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(IdentityConstants.IdClaim);
            return int.TryParse(userId, out var userIdValue)
                ? userService.GetUserRoleDetails(userIdValue)
                : Task.FromResult<IEnumerable<RoleAssignment>>(null);
        }

        /// <summary>
        /// Extracts a KIS refresh token from the specified <paramref name="principal"/>. Exchanges it for
        /// a new KIS authentication token, fetches KIS user data, synchronizes the user with the local database
        /// and returns a JWT Bearer token representing the user's identity.
        /// </summary>
        /// <param name="userService">An <see cref="IUserService"/> instance.</param>
        /// <param name="principal">The principal that contains a 'kisrt' claim.</param>
        /// <returns>A <see cref="LoginResult"/> structure with the JWT Bearer token or information about errors.</returns>
        public static Task<LoginResult> Refresh(this IUserService userService, ClaimsPrincipal principal)
        {
            var refreshToken = principal.FindFirstValue(IdentityConstants.KisRefreshTokenClaim);
            if (refreshToken is null)
                return null;

            return userService.LoginToken(refreshToken);
        }

        public static async Task<MadeByUserDto> GetUserMadeByDto(this IUserService userService, int? userId,
            bool showId)
        {
            if (!userId.HasValue)
                return null;

            var user = await userService.GetUser(userId.Value);
            if (user is null)
                return null;

            var dto = new MadeByUserDto()
            {
                Name = user.Name,
                Nickname = user.Nickname,
                Email = user.Email,
                Id = showId ? user.Id : null
            };

            return dto;
        }
    }
}
