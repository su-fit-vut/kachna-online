// ReservationItemEvent.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Models.BoardGames
{
    /// <summary>
    /// Model representing a single change in a reservation item's state.
    /// </summary>
    public class ReservationItemEvent
    {
        /// <summary>
        /// ID of the item that the event modifies.
        /// </summary>
        public int ReservationItemId { get; set; }
        /// <summary>
        /// Who performed the change.
        /// </summary>
        public int MadeById { get; set; }
        /// <summary>
        /// When the change was performed.
        /// </summary>
        public DateTime MadeOn { get; set; }
        /// <summary>
        /// New overall state of the reservation after the change.
        /// </summary>
        public ReservationItemState NewState { get; set; }
        /// <summary>
        /// Type of the change.
        /// </summary>
        public ReservationEventType Type { get; set; }
        /// <summary>
        /// If extension was granted, the items' new expiration date.
        /// </summary>
        public DateTime? NewExpiryDateTime { get; set; }
    }
}
