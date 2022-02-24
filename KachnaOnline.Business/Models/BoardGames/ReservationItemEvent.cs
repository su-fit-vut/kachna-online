using System;

namespace KachnaOnline.Business.Models.BoardGames
{
    /// <summary>
    /// Model representing a single change in a reservation item's state.
    /// </summary>
    public class ReservationItemEvent
    {
        /// <summary>
        /// Creates a new event.
        /// </summary>
        /// <param name="type">Type of the event.</param>
        /// <param name="reservationItemId">ID of the reservation item this event ties to.</param>
        /// <param name="madeById">ID of the user that requested the event.</param>
        /// <param name="newExpiryDateTime">New expiration.</param>
        public ReservationItemEvent(ReservationEventType type, int reservationItemId, int madeById,
            DateTime? newExpiryDateTime = null)
        {
            this.ReservationItemId = reservationItemId;
            this.MadeById = madeById;
            this.MadeOn = DateTime.Now;
            this.Type = type;
            switch (this.Type)
            {
                case ReservationEventType.Created:
                    this.NewState = ReservationItemState.New;
                    break;
                case ReservationEventType.Assigned:
                    this.NewState = ReservationItemState.Assigned;
                    break;
                case ReservationEventType.Cancelled:
                    this.NewState = ReservationItemState.Cancelled;
                    break;
                case ReservationEventType.Returned:
                    this.NewState = ReservationItemState.Done;
                    break;
                default:
                    // HandedOver, ExtensionRequest, ExtensionRefused, ExtensionGranted all keep the overall state as
                    // HandedOver.
                    this.NewState = ReservationItemState.HandedOver;
                    break;
            }

            this.NewExpiryDateTime = newExpiryDateTime;
        }

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
