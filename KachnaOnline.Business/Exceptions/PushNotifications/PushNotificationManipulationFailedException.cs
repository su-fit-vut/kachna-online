using System;

namespace KachnaOnline.Business.Exceptions.PushNotifications
{
    /// <summary>
    /// Thrown when a manipulation with a push notification fails (e.g. database error).
    /// </summary>
    public class PushNotificationManipulationFailedException : Exception
    {
        public PushNotificationManipulationFailedException() : base("Manipulation with a push notification failed.")
        {
        }
    }
}
