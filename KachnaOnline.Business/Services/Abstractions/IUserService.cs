// IUserService.cs
// Author: Ondřej Ondryáš

using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Models.Users;

namespace KachnaOnline.Business.Services.Abstractions
{
    public interface IUserService
    {
        /// <summary>
        /// Exchanges a KIS session ID for a KIS authentication token, fetches KIS user data, synchronizes
        /// the user with the local database and returns a JWT Bearer token representing the user's identity.
        /// </summary>
        /// <param name="kisSessionId">A KIS session ID.</param>
        /// <returns>A <see cref="LoginResult"/> structure with the JWT Bearer token or information about errors.</returns>
        Task<LoginResult> LoginSession(string kisSessionId);

        /// <summary>
        /// Exchanges a KIS refresh token for a new KIS authentication token, fetches KIS user data, synchronizes
        /// the user with the local database and returns a JWT Bearer token representing the user's identity.
        /// </summary>
        /// <remarks>
        /// If KIS returns a 403 Forbidden (login not allowed) response, it means that the user is not a member anymore.
        /// In that case, a <see cref="LoginResult"/> with <see cref="LoginResult.UserFound"/> set to false is returned.
        /// </remarks>
        /// <param name="kisRefreshToken">A KIS refresh token.</param>
        /// <returns>A <see cref="LoginResult"/> structure with the JWT Bearer token or information about errors.</returns>
        Task<LoginResult> LoginToken(string kisRefreshToken);

        /// <summary>
        /// Returns the user corresponding to the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>A <see cref="User"/> object containing the user matching the specified <paramref name="userId"/>
        /// if such user exists, or null if it doesn't.</returns>
        Task<User> GetUser(int userId);

        /// <summary>
        /// Returns the names of roles assigned to the user corresponding to the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>An enumerable of names of roles assigned to the user matching the specified <paramref name="userId"/>
        /// if such user exists, or null if it doesn't.</returns>
        Task<IEnumerable<string>> GetUserRoles(int userId);

        /// <summary>
        /// Returns <see cref="RoleAssignment"/> objects describing the role assignments for the user corresponding
        /// to the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>An enumerable of <see cref="RoleAssignment"/> objects describing the role assignments for the user
        /// matching the specified <paramref name="userId"/> if such user exists, or null if it doesn't.</returns>
        Task<IEnumerable<RoleAssignment>> GetUserRoleDetails(int userId);

        /// <summary>
        /// Checks whether the user corresponding to the specified <paramref name="userId"/> has the role specified
        /// by <paramref name="role"/>. 
        /// </summary>
        /// <param name="userId">The user ID to check the role for.</param>
        /// <param name="role">The name of the role to check.</param>
        /// <returns>Null if such user doesn't exist. True if the user has the specified role. False otherwise.</returns>
        Task<bool?> IsInRole(int userId, string role);
    }
}
