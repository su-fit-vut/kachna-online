// IPushSubscriptionsService.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Models.PushNotifications;
using KachnaOnline.Business.Exceptions.PushNotifications;

namespace KachnaOnline.Business.Services.Abstractions
{
    public interface IPushSubscriptionsService
    {
        /// <summary>
        /// Creates or updates an existing user push subscription.
        /// </summary>
        /// <param name="subscription">The subscription model to create or update.</param>
        /// <exception cref="PushNotificationManipulationFailedException">When the subscription failed.</exception>
        Task CreateOrUpdateSubscription(PushSubscription subscription);

        /// <summary>
        /// Deletes all push subscriptions for a given endpoint.
        /// </summary>
        /// <param name="endpoint">Subscription endpoint to delete.</param>
        /// <exception cref="PushNotificationManipulationFailedException">When the deletion failed.</exception>
        Task DeletePushSubscription(string endpoint);

        /// <summary>
        /// Returns an existing anonymous subscription based on the subscription endpoint.
        /// </summary>
        /// <param name="endpoint">Endpoint of the subscription to get.</param>
        /// <returns>A push subscription with <paramref name="endpoint"/> or null if it does not exist.</returns>
        Task<PushSubscription> GetPushSubscription(string endpoint);

        /// <summary>
        /// Returns a collection of subscriptions which have state change notification enabled.
        /// </summary>
        /// <returns>A collection of subscriptions to state change notification.</returns>
        Task<ICollection<PushSubscription>> GetStateChangesEnabledSubscriptions();

        /// <summary>
        /// Returns a collection of subscriptions which have board games notification of a particular user enabled.
        /// </summary>
        /// <param name="userId">ID of the user to get subscription of of.</param>
        /// <returns>A collection of subscriptions on which a user with <paramref name="userId"/> is subscribed to
        /// board games notifications.</returns>
        Task<ICollection<PushSubscription>> GetUserBoardGamesEnabledSubscriptions(int userId);
    }
}
