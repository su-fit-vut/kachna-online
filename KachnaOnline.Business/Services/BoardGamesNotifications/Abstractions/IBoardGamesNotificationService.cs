// IBoardGamesNotificationService.cs
// Author: František Nečas

using System.Threading.Tasks;

namespace KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions
{
    /// <summary>
    /// Represents a service that performs actions when the state of a board games reservations changes
    /// or is about to change. This includes sending emails, Discord messages etc.
    /// </summary>
    public interface IBoardGamesNotificationService
    {
        /// <summary>
        /// Performs actions for reservation creation.
        /// </summary>
        /// <param name="reservationId">ID of the newly created reservation.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerReservationCreated(int reservationId);

        /// <summary>
        /// Performs actions for reservation full assignment..
        /// </summary>
        /// <param name="reservationId">ID of the fully assigned reservation.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerReservationFullyAssigned(int reservationId);

        /// <summary>
        /// Performs actions for request of reservation extension.
        /// </summary>
        /// <param name="itemId">ID of the reservation item where extension was requested.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerReservationItemExtensionRequest(int itemId);

        /// <summary>
        /// Performs actions for a reservation item expiring soon (e.g. in 3 days).
        /// </summary>
        /// <param name="itemId">ID of the reservation item that is about to expire.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerReservationItemExpiresSoon(int itemId);

        /// <summary>
        /// Performs actions for a reservation item expiration.
        /// </summary>
        /// <param name="itemId">ID of the reservation item that just expired.</param>
        /// <returns><see cref="Task"/> that represents the asynchronous operation.</returns>
        Task TriggerReservationItemExpired(int itemId);
    }
}
