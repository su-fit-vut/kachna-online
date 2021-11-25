// UserRoleAssignmentDetailsDto.cs
// Author: Ondřej Ondryáš

namespace KachnaOnline.Dto.Users
{
    public class UserRoleAssignmentDetailsDto
    {
        /// <summary>
        /// Details of the administrator that has created this role assignment.
        /// May be null if this assignment was mapped from a KIS role.
        /// </summary>
        public UserDto AssignedBy { get; set; }

        /// <summary>
        /// The role name.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// If true, the role has been revoked from the user manually by an administrator.
        /// <see cref="AssignedBy"/> will contain information about the administrator.
        /// </summary>
        public bool ManuallyDisabled { get; set; }

        /// <summary>
        /// Flag signalising whether this role assignment was made manually by an administrator
        /// (true) or automatically by mapping a KIS role (false).
        /// </summary>
        public bool AssignedManually => this.AssignedBy != null;
    }
}
