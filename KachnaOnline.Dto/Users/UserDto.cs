// UserDto.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.Users
{
    /// <summary>
    /// Represents a single user.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Unique ID of the user.
        /// </summary>
        /// <example>5</example>
        public int Id { get; set; }

        /// <summary>
        /// Name of the user.
        /// </summary>
        /// <example>František Nečas</example>
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Nickname of the user.
        /// </summary>
        /// <example>Fifinas</example>
        [StringLength(128)]
        public string Nickname { get; set; }

        /// <summary>
        /// Email of the user.
        /// </summary>
        /// <example>foo@bar.cz</example>
        public string Email { get; set; }
    }
}
