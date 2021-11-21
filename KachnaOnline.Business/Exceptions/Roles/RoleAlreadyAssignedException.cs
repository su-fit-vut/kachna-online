// RoleAlreadyAssignedException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.Roles
{
    /// <summary>
    /// Thrown when the requested role has already been assigned to the user.
    /// </summary>
    public class RoleAlreadyAssignedException : Exception
    {
        public RoleAlreadyAssignedException(int userId, int roleId) : base(
            $"Role with id {roleId} has previously been assigned to user with ID {userId}.")
        {
        }
    }
}
