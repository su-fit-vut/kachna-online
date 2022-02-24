using System;

namespace KachnaOnline.Business.Exceptions.Roles
{
    /// <summary>
    /// Thrown when a requested role does not exist.
    /// </summary>
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException() : base("Requested role was not found.")
        {
        }

        public RoleNotFoundException(int roleId) : base($"Role with ID {roleId} was not found.")
        {
        }
    }
}
