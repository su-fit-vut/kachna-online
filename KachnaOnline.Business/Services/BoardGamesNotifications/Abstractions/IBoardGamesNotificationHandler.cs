// IBoardGamesNotificationHandler.cs
// Author: František Nečas

using System.Threading.Tasks;

namespace KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions
{
    /// <summary>
    /// Represents a single type of action that happens when a board game stata changes or is about to change.
    /// </summary>
    public interface IBoardGamesNotificationHandler
    {
        /// <summary>
        /// Performs this handler's action for reservation creation.
        /// </summary>
        /// <param name="reservationId">ID of the newly created reservation.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformReservationCreated(int reservationId);
        
        /// <summary>
        /// Performs this handler's action for reservation full assignment.
        /// </summary>
        /// <param name="reservationId">ID of the fully assigned reservation.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformReservationFullyAssigned(int reservationId);

        /// <summary>
        /// Performs this handler's action for request of reservation extension.
        /// </summary>
        /// <param name="itemId">ID of the reservation item where extension was requested.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformReservationItemExtensionRequest(int itemId);

        /// <summary>
        /// Performs this handler's for a reservation item expiring soon (e.g. in 3 days).
        /// </summary>
        /// <param name="itemId">ID of the reservation item that is about to expire.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformReservationItemExpiresSoon(int itemId);

        /// <summary>
        /// Performs this handler's action for a reservation item expiration.
        /// </summary>
        /// <param name="itemId">ID of the reservation item that just expired.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task PerformReservationItemExpired(int itemId);
    }
}
