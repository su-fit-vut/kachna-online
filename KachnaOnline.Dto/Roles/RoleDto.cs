// RoleDto.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.Roles
{
    public class RoleDto
    {
        /// <summary>
        /// Unique ID of the role.
        /// </summary>
        /// <example>3</example>
        public int Id { get; set; }

        /// <summary>
        /// Textual name of the role.
        /// </summary>
        /// <example>BoardGamesManager</example>
        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
        public string Name { get; set; }
    }
}
