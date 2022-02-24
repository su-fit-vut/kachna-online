using System;

namespace KachnaOnline.Business.Exceptions.Events
{
    public class LinkingStateToEventException : Exception
    {
        public int? StateId { get; }
        public int? EventId { get; }

        public LinkingStateToEventException(string message) : base(message)
        {
        }

        public LinkingStateToEventException(string message, int eventId, int stateId) : base(message)
        {
            this.StateId = stateId;
            this.EventId = eventId;
        }

        public LinkingStateToEventException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LinkingStateToEventException(string message, Exception innerException, int eventId, int stateId) : base(message,
            innerException)
        {
            this.StateId = stateId;
            this.EventId = eventId;
        }
    }
}
