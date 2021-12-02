// PushSubscriptionConfiguration.cs
// Author: František Nečas

namespace KachnaOnline.Dto.PushNotifications
{
    /// <summary>
    /// Contains information about which push notifications are enabled.
    /// </summary>
    public class PushSubscriptionConfiguration
    {
        /// <summary>
        /// Whether notifications about club state changes are enabled.
        /// </summary>
        public bool StateChangesEnabled { get; set; } = false;

        /// <summary>
        /// Whether notifications about board games reservations are enabled.
        /// </summary>
        public bool? BoardGamesEnabled { get; set; }
    }
}
