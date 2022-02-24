using System;

namespace KachnaOnline.Business.Exceptions.ClubStates
{
    public class StateReadOnlyException : Exception
    {
        public int StateId { get; }

        public StateReadOnlyException(int stateId) : base(
            "Cannot modify the specified state because it has already started or ended.")
        {
            this.StateId = stateId;
        }
    }
}
