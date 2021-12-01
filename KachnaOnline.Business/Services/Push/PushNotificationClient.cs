// PushNotificationClient.cs
// Author: František Nečas

using System.Threading.Tasks;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Models.PushNotifications;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Options;
using PushSubscription = KachnaOnline.Business.Models.PushNotifications.PushSubscription;

namespace KachnaOnline.Business.Services.Push
{
    public abstract class PushNotificationClient
    {
        private readonly PushServiceClient _pushClient;

        protected PushNotificationClient(IOptionsMonitor<PushOptions> pushOptions, PushServiceClient pushClient)
        {
            _pushClient = pushClient;
            _pushClient.DefaultAuthentication = new VapidAuthentication(pushOptions.CurrentValue.PublicKey,
                pushOptions.CurrentValue.PrivateKey)
            {
                Subject = pushOptions.CurrentValue.Subject
            };
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
