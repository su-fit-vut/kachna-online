using System;

namespace KachnaOnline.Business.Exceptions.ClubStates
{
    public class StateNotAssociatedToEventException : Exception
    {
        public int? StateId { get; }

        public StateNotAssociatedToEventException() : base("The specified state is not associated to any event.")
        {
        }

        public StateNotAssociatedToEventException(int stateId) : base($"State with ID {stateId} is not associated to any event.")
        {
            this.StateId = stateId;
        }

        public StateNotAssociatedToEventException(string message) : base(message)
        {
        }
    }
}
