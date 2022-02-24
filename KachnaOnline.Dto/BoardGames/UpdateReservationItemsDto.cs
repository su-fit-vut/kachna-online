using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents items to be added to a reservation.
    /// </summary>
    public class UpdateReservationItemsDto
    {
        /// <summary>
        /// IDs of games to add to a reservation. If multiple copies of a game are to be reserved,
        /// its ID must be include multiple times. Must not be empty.
        /// </summary>
        /// <example>[3, 3]</example>
        [Required]
        [MinLength(1)]
        public int[] BoardGameIds { get; set; }
    }
}
