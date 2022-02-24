using System.Collections.Generic;

namespace KachnaOnline.Business.Models.PushNotifications
{
    /// <summary>
    /// Represents a push subscription.
    /// </summary>
    public class PushSubscription : Lib.Net.Http.WebPush.PushSubscription
    {
        /// <summary>
        /// ID of the user who made the subscription if it was made by a user.
        /// </summary>
        public int? MadeById { get; set; }

        /// <summary>
        /// Whether board games reservation subscriptions are enabled.
        /// </summary>
        public bool? BoardGamesEnabled { get; set; }

        /// <summary>
        /// Whether state changes notifications are enabled.
        /// </summary>
        public bool StateChangesEnabled { get; set; }
    }
}
