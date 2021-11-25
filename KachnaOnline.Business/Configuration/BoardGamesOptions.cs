// BoardGamesOptions.cs
// Author: František Nečas

namespace KachnaOnline.Business.Configuration
{
    public class BoardGamesOptions
    {
        /// <summary>
        /// The number of days that a reservation lasts unless the game specifies otherwise.
        /// </summary>
        public int DefaultReservationDays { get; set; } = 30;

        /// <summary>
        /// The number of days for which a reservation is extended.
        /// </summary>
        public int ExtensionDays { get; set; } = 7;

        /// <summary>
        /// How often (in number of minutes) the background notification service should check for new
        /// events to notify about.
        /// </summary>
        public int NotificationServiceIntervalMinutes { get; set; } = 5;

        /// <summary>
        /// Number of days before expiration when the user is informed about an upcoming reservation expiration.
        /// </summary>
        public int NotifyBeforeExpirationDays { get; set; } = 3;

        /// <summary>
        /// Webhook URL to Student Union Discord for board games related information.
        /// </summary>
        public string SuDiscordWebhookUrl { get; set; }
    }
}
