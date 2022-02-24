using System;
using KachnaOnline.Dto.Swagger;
using KachnaOnline.Dto.Users;

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Represents a single change of an item state in a reservation.
    /// </summary>
    public class ReservationItemEventDto
    {
        /// <summary>
        /// Details about the user who made this change.
        /// </summary>
        [SwaggerNotNull]
        public MadeByUserDto MadeBy { get; set; }

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
