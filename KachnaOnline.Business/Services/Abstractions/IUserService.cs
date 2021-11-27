// IUserService.cs
// Author: Ondřej Ondryáš

using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Models.Users;
using KachnaOnline.Business.Exceptions.Roles;
using KachnaOnline.Business.Exceptions;

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
        /// Returns a user with the given <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>A <see cref="User"/> object containing the user matching the specified <paramref name="userId"/>
        /// if such user exists, or null if it doesn't.</returns>
        Task<User> GetUser(int userId);

        /// <summary>
        /// Modifies the mutable properties of a user.
        /// </summary>
        /// <param name="user">The user model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        /// <exception cref="UserNotFoundException">The user does not exist.</exception>
        Task SaveUser(User user);

        /// <summary>
        /// Returns the names of roles assigned to a user with the given <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>An enumerable of names of roles assigned to the user matching the specified <paramref name="userId"/>
        /// if such user exists, or null if it doesn't.</returns>
        Task<IEnumerable<string>> GetUserRoles(int userId);

        /// <summary>
        /// Returns <see cref="RoleAssignment"/> objects describing the role assignments for a user with the given
        /// <paramref name="userId"/>. Role assignments describing disabled roles are also returned.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>An enumerable of <see cref="RoleAssignment"/> objects describing the role assignments for the user
        /// matching the specified <paramref name="userId"/> if such user exists, or null if it doesn't.</returns>
        Task<IEnumerable<RoleAssignment>> GetUserRoleDetails(int userId);

        /// <summary>
        /// Checks if a user with the given <paramref name="userId"/> has a role specified by <paramref name="role"/>.
        /// </summary>
        /// <param name="userId">The user ID to check the role for.</param>
        /// <param name="role">The name of the role to check.</param>
        /// <returns>Null if such user doesn't exist. True if the user has the specified role. False otherwise.</returns>
        Task<bool?> IsInRole(int userId, string role);

        /// <summary>
        /// Returns all roles present in the system.
        /// </summary>
        /// <returns>An enumerable of all roles.</returns>
        Task<IEnumerable<string>> GetRoles();

        /// <summary>
        /// Returns all users.
        /// </summary>
        /// <returns>An enumerable of all users.</returns>
        Task<IEnumerable<User>> GetUsers();

        /// <summary>
        /// Assigns a role to a user manually.
        /// </summary>
        /// <remarks>
        /// After performing this operation, the user will always obtain the role when logging in, even if they
        /// should not have it based on a KIS mapping.
        /// This forceful assignment can be reversed by calling <see cref="ResetRole"/>.
        /// </remarks>
        /// <param name="userId">The ID of the user to assign the role to.</param>
        /// <param name="role">The name of the role to assign.</param>
        /// <param name="assignedByUserId">The ID of a user that performed the operation.</param>
        /// <exception cref="RoleNotFoundException">The role does not exist.</exception>
        /// <exception cref="RoleManipulationFailedException">The assignment failed.</exception>
        /// <exception cref="UserNotFoundException">The user does not exist.</exception>
        Task AssignRole(int userId, string role, int assignedByUserId);

        /// <summary>
        /// Revokes a role from a user manually.
        /// </summary>
        /// <remarks>
        /// After performing this operation, the user will never obtain the role when logging in, even if they
        /// should have it based on a KIS mapping.
        /// This forceful revocation can be reversed by calling <see cref="ResetRole"/>.
        /// </remarks>
        /// <param name="userId">The ID of the user to revoke the role from.</param>
        /// <param name="role">The name of the role to revoke from the user.</param>
        /// <param name="revokedByUserId">The ID of a user that performed the operation.</param>
        /// <exception cref="RoleNotFoundException">The role does not exist.</exception>
        /// <exception cref="RoleManipulationFailedException">The revocation failed.</exception>
        /// <exception cref="UserNotFoundException">The user does not exist.</exception>
        Task RevokeRole(int userId, string role, int revokedByUserId);

        /// <summary>
        /// Resets a manual role assignment.
        /// </summary>
        /// <remarks>
        /// If the user does not have an existing role assignment to the specified role (or the role does not exist
        /// at all), this method does nothing.
        /// </remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="role">The name of the role.</param>
        /// <param name="resetByUserId">The ID of a user that performed the operation.</param>
        /// <exception cref="RoleManipulationFailedException">The operation failed.</exception>
        /// <exception cref="UserNotFoundException">The user does not exist.</exception>
        Task ResetRole(int userId, string role, int resetByUserId);
    }
}
