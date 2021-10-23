// StatePlanningException.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Exceptions.ClubStates
{
    public class StatePlanningException : Exception
    {
        public int? StateId { get; }

        public StatePlanningException(string message) : base(message)
        {
        }

        public StatePlanningException(string message, int stateId) : base(message)
        {
            this.StateId = stateId;
        }

        public StatePlanningException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StatePlanningException(string message, Exception innerException, int stateId) : base(message,
            innerException)
        {
            this.StateId = stateId;
        }
    }
}
