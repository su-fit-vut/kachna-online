// ReservationItem.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Models.BoardGames
{
    /// <summary>
    /// Model representing a single item (board game) in a reservation.
    /// </summary>
    public class ReservationItem
    {
        /// <summary>
        /// Unique ID of the item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the reservation that the item belongs to.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// ID of the board game that the item reserves.
        /// </summary>
        public int BoardGameId { get; set; }

        /// <summary>
        /// Expiration date of the item. May be null before the game is handed over.
        /// </summary>
        public DateTime? ExpiresOn { get; set; }
    }
}
