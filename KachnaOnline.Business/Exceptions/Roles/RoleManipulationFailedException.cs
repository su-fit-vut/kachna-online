using System;

namespace KachnaOnline.Business.Exceptions.Roles
{
    /// <summary>
    /// Thrown when a manipulation with a role fails (e.g. database throws an error).
    /// </summary>
    public class RoleManipulationFailedException : Exception
    {
        public RoleManipulationFailedException() : base("Manipulation with a role failed.")
        {
        }
    }
}
