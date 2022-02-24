using System;

namespace KachnaOnline.Business.Exceptions.PushNotifications
{
    /// <summary>
    /// Thrown when a key required for push notifications is missing in the config.
    /// </summary>
    public class KeysNotAvailableException : Exception
    {
        public KeysNotAvailableException() : base("Private and public VAPID keys must be provided.")
        {
        }
    }
}
