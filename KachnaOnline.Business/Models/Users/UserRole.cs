// UserRole.cs
// Author: František Nečas

namespace KachnaOnline.Business.Models.Users
{
    /// <summary>
    /// Model representing an assignment of role to a user.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// ID of the user who the role is assigned to.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ID of the assigned role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Whether the role should be taken into consideration.
        /// </summary>
        public bool ForceDisable { get; set; } = false;

        /// <summary>
        /// ID of the user who assigned the role.
        /// </summary>
        public int? AssignedByUserId { get; set; }
    }
}
