// ManagerReservationDto.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a single board games reservation as seen by a board games manager.
    /// </summary>
    public class ManagerReservationDto : ReservationDto
    {
        /// <summary>
        /// An internal note.
        /// </summary>
        /// <example>Created by LeO.</example>
        [StringLength(1024)]
        public string NoteInternal { get; set; }
    }
}
