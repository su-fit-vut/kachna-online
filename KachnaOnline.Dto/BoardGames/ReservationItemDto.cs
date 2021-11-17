// ReservationItemDto.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents an item of a reservation.
    /// </summary>
    public class ReservationItemDto
    {
        /// <summary>
        /// ID of the item.
        /// </summary>
        /// <example>3</example>
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the game which this item reserves.
        /// </summary>
        /// <example>55</example>
        public int BoardGameId { get; set; }
        
        /// <summary>
        /// Expiration date of the reservation item.
        /// May be null if it is not set yet (i.e. the game has not been handed over).
        /// </summary>
        public DateTime? ExpiresOn { get; set; }
    }
}
