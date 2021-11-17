// ReservationNoteInternalDto.cs
// Author: František Nečas

using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Carries an internal note.
    /// </summary>
    public class ReservationNoteInternalDto
    {
        /// <summary>
        /// An internal note.
        /// </summary>
        /// <example>Do not extend anymore.</example>
        [StringLength(1024)]
        public string NoteInternal { get; set; }
    }
}
