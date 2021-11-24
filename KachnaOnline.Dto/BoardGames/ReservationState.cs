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
        /// Reservation is still in progress (complementary to <see cref="Done"/>).
        /// </summary>
        Current = 1,

        /// <summary>
        /// Games have been returned or the whole reservation has been cancelled.
        /// </summary>
        Done = 2,

        /// <summary>
        /// At least one item in the whole reservation has expired borrowing period.
        /// </summary>
        Expired = 3
    }
}
