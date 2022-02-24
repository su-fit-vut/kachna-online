using System;

namespace KachnaOnline.Business.Exceptions.ClubStates
{
    public class StateNotFoundException : Exception
    {
        public int? StateId { get; }

        public StateNotFoundException() : base("The specified state does not exist.")
        {
        }

        public StateNotFoundException(int stateId) : base($"No state with ID {stateId} exists.")
        {
            this.StateId = stateId;
        }

        public StateNotFoundException(string message) : base(message)
        {
        }
    }
}
