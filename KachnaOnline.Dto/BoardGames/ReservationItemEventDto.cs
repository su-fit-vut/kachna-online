// ReservationItemEventDto.cs
// Author: František Nečas

using System;
using System.ComponentModel.DataAnnotations;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a single change of an item state in a reservation.
    /// </summary>
    public class ReservationItemEventDto
    {
        /// <summary>
        /// The creator of the change in state.
        /// </summary>
        public int MadeById { get; set; }

        /// <summary>
        /// When the change was done.
        /// </summary>
        public DateTime MadeOn { get; set; }

        /// <summary>
        /// New overall state of the reservation item.
        /// </summary>
        public ReservationItemState NewState { get; set; }

        /// <summary>
        /// Type of change.
        /// </summary>
        public ReservationEventType Type { get; set; }

        /// <summary>
        /// Optional new expiration date for when extension is granted.
        /// </summary>
        public DateTime? NewExpiryDateTime { get; set; }
    }
}
