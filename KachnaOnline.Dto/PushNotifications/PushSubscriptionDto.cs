using System.ComponentModel.DataAnnotations;
using Lib.Net.Http.WebPush;

namespace KachnaOnline.Dto.PushNotifications
{
    public class PushSubscriptionDto
    {
        /// <summary>
        /// The subscription data.
        /// </summary>
        [Required]
        public PushSubscription Subscription { get; set; }

        /// <summary>
        /// Configuration of the subscription.
        /// </summary>
        [Required]
        public PushSubscriptionConfiguration Configuration { get; set; }
    }
}
