namespace KachnaOnline.Dto.Users
{
    public class UserDetailsDto : UserDto
    {
        /// <summary>
        /// Optional Discord ID of the user.
        /// </summary>
        /// <example>4378291</example>
        public ulong? DiscordId { get; set; }

        /// <summary>
        /// Roles that the user has.
        /// </summary>
        /// <example>["BoardGamesManager", "Admin"]</example>
        public string[] ActiveRoles { get; set; }

        /// <summary>
        /// Detailed information about the user's role assignments.
        /// </summary>
        public UserRoleAssignmentDetailsDto[] ManuallyAssignedRoles { get; set; }
    }
}
