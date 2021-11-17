// ReservationState.cs
// Author: František Nečas

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// Overall state of the reservation.
    /// </summary>
    public enum ReservationState
    {
        /// <summary>
        /// Newly created reservation, at least one item has not been assigned.
        /// </summary>
        New = 0,

        /// <summary>
        /// User cancelled the reservation before it was processed.
        /// </summary>
        Cancelled = 1,

        /// <summary>
        /// Assigned to a board games manager.
        /// </summary>
        Assigned = 2,

        /// <summary>
        /// User already has received the games.
        /// </summary>
        HandedOver = 3,

        /// <summary>
        /// Games have been returned.
        /// </summary>
        Done = 4,
        
        /// <summary>
        /// At least one item in the whole reservation has expired borrowing period.
        /// </summary>
        Expired = 5,
    }
}
