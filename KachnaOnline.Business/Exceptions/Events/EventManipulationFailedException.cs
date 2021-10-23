// EventManipulationFailedException.cs
// Author: David Chocholatý

using System;

namespace KachnaOnline.Business.Exceptions.Events
{
    /// <summary>
    /// Thrown when a manipulation (e.g. planning or deleting) with an event fails (e.g. due to a database error).
    /// </summary>
    public class EventManipulationFailedException : Exception
    {
        private int? EventId { get; }

        public EventManipulationFailedException() : base("Operation with the given event failed.")
        {
        }

        public EventManipulationFailedException(string message) : base(message)
        {
        }

        public EventManipulationFailedException(string message, int eventId) : base(message)
        {
            this.EventId = eventId;
        }

        public EventManipulationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EventManipulationFailedException(string message, Exception innerException, int eventId) : base(message,
            innerException)
        {
            this.EventId = eventId;
        }
    }
}
