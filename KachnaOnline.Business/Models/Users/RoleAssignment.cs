// RoleAssignment.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Business.Models.Users
{
    public class RoleAssignment
    {
        /// <summary>
        /// A <see cref="User"/> object with information about the administrator that has
        /// created this role assignment. May be null if this assignment was mapped from a KIS role.
        /// </summary>
        public User AssignedBy { get; set; }
        
        /// <summary>
        /// The role name.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Flag signalising whether this role assignment was made manually by an administrator
        /// (true) or automatically by mapping a KIS role (false).
        /// </summary>
        public bool AssignedManually => this.AssignedBy != null;
    }
}
