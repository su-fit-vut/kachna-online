// PushNotificationClient.cs
// Author: František Nečas

using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Models.PushNotifications;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PushSubscription = KachnaOnline.Business.Models.PushNotifications.PushSubscription;

namespace KachnaOnline.Business.Services.Push
{
    public abstract class PushNotificationClient
    {
        private readonly PushServiceClient _pushClient;
        private readonly ILogger<PushNotificationClient> _logger;
        private readonly IOptionsMonitor<PushOptions> _options;

        protected PushNotificationClient(IOptionsMonitor<PushOptions> pushOptions, PushServiceClient pushClient,
            ILogger<PushNotificationClient> logger)
        {
            _pushClient = pushClient;
            _logger = logger;
            _options = pushOptions;
            if (string.IsNullOrEmpty(pushOptions.CurrentValue.PublicKey) ||
                string.IsNullOrEmpty(pushOptions.CurrentValue.PrivateKey))
            {
                logger.LogWarning("VAPID keys are not set up, push notifications will not go through.");
            }
            else
            {
                _pushClient.DefaultAuthentication = new VapidAuthentication(pushOptions.CurrentValue.PublicKey,
                    pushOptions.CurrentValue.PrivateKey)
                {
                    Subject = pushOptions.CurrentValue.Subject
                };
            }
        }

        /// <summary>
        /// Sends a push notification.
        /// </summary>
        /// <param name="subscription">An active push subscription.</param>
        /// <param name="title">Title of the push notification.</param>
        /// <param name="body">Body of the push notification.</param>
        /// <param name="icon">URL to the icon of the push notification.</param>
        protected async Task SendNotification(PushSubscription subscription, string title, string body,
            string icon = "assets/kachna_bez_peny.png")
        {
            if (string.IsNullOrEmpty(_options.CurrentValue.PublicKey) ||
                string.IsNullOrEmpty(_options.CurrentValue.PrivateKey))
            {
                _logger.LogWarning("VAPID keys are not set up, not sending push notification.");
            }

            PushMessage notification = new AngularPushNotification
            {
                Title = title,
                Body = body,
                Icon = icon
            }.ToPushMessage();
            await _pushClient.RequestPushMessageDeliveryAsync(subscription, notification);
        }
    }
}
