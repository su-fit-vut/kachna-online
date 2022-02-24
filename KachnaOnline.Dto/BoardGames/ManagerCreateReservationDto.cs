using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a reservation to be created by a board games manager for a regular user
    /// (e.g. right in the club without prior notice).
    /// </summary>
    public class ManagerCreateReservationDto : CreateReservationDto
    {
        /// <summary>
        /// An internal note.
        /// </summary>
        /// <example>Created by LeO.</example>
        [StringLength(1024)]
        public string NoteInternal { get; set; }
    }
}
