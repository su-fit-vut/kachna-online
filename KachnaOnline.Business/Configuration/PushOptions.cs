// PushOptions.cs
// Author: František Nečas

namespace KachnaOnline.Business.Configuration
{
    public class PushOptions
    {
        /// <summary>
        /// VAPID public key for use in push notifications.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// VAPID private key for use in push notifications.
        /// </summary>
        public string PrivateKey { get; set; }
    }
}
