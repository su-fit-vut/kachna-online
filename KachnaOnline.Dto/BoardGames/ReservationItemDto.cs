// ReservationItemDto.cs
// Author: František Nečas

using System;
using KachnaOnline.Dto.Users;

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

        /// <summary>
        /// Details about the board games manager whom this item is assigned to. May be null if
        /// it is not set yet (i.e. has not been assigned).
        /// </summary>
        public MadeByUserDto AssignedTo { get; set; }

        /// <summary>
        /// Current state of the reservation item.
        /// </summary>
        public ReservationItemState State { get; set; }
    }
}
