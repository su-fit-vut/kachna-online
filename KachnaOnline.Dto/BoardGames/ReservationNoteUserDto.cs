// ReservationNoteUserDto.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Carries a user note.
    /// </summary>
    public class ReservationNoteUserDto
    {
        /// <summary>
        /// A user note.
        /// </summary>
        /// <example>For an upcoming party.</example>
        [Required]
        [StringLength(1024)]
        public string NoteUser { get; set; }
    }
}
