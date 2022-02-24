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
        /// Information about the reserved board game.
        /// </summary>
        public ReservedBoardGameDto BoardGame { get; set; }

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

        /// <summary>
        /// Type of the last event performed to this item.
        /// </summary>
        /// <remarks>
        /// Semantics of this property are a bit different from <see cref="State"/>. Unlike <see cref="State"/>,
        /// this property allows fully determining which actions can be performed with the item, whereas
        /// <see cref="State"/> is only a subset of overall item states and information such as whether extension
        /// was requested cannot be distinguished.
        /// </remarks>
        public ReservationEventType LastEventType { get; set; }
    }
}
