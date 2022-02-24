using System;

namespace KachnaOnline.Business.Exceptions.Events
{
    /// <summary>
    /// Thrown when an event was not found (e.g. when a wrong ID has been passed in a request).
    /// </summary>
    public class EventNotFoundException : Exception
    {
        public EventNotFoundException() : base("The specified event does not exist.")
        {
        }
    }
}
