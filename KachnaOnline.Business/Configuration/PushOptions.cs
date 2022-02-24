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

        /// <summary>
        /// Subject of push notifications (must be https URI or mailto address).
        /// </summary>
        public string Subject { get; set; }
    }
}
