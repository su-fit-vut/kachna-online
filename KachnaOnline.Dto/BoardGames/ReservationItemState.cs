// ReservationItemState.cs
// Author: František Nečas

namespace KachnaOnline.Dto.BoardGames
{
    /// <summary>
    /// State of a single reservation item.
    /// </summary>
    public enum ReservationItemState
    {
        /// <summary>
        /// Newly created reservation item.
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
        /// User has already received the game.
        /// </summary>
        HandedOver = 3,

        /// <summary>
        /// The game has been returned.
        /// </summary>
        Done = 4,

        /// <summary>
        /// The game's reservation has expired.
        /// </summary>
        Expired = 5,
    }
}
