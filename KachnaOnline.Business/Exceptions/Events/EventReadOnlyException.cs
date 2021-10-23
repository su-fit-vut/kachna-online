using System;

namespace KachnaOnline.Business.Exceptions.Events
{
    /// <summary>
    /// Thrown when an event cannot be modified or removed, because the event has already ended.
    /// </summary>
    public class EventReadOnlyException : Exception
    {
        public EventReadOnlyException() :
            base("Cannot modify an event that has already ended.")
        {
        }
    }
}
