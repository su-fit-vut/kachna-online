// CreateReservationDto.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a reservation to be created by a regular user.
    /// </summary>
    public class CreateReservationDto
    {
        /// <summary>
        /// A user note.
        /// </summary>
        /// <example>For an upcoming party.</example>
        [StringLength(1024)]
        public string NoteUser { get; set; }

        /// <summary>
        /// IDs of games to reserve. If multiple copies of a game are to be reserved, its ID must be included
        /// multiple times. Must not be empty.
        /// </summary>
        /// <example>[1, 1, 2]</example>
        [Required]
        [MinLength(1)]
        public int[] BoardGameIds { get; set; }
    }
}
