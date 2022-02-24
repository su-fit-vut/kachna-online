using System;

namespace KachnaOnline.Business.Exceptions.ClubStates
{
    public class RepeatingStateReadOnlyException : Exception
    {
        public int RepeatingStateId { get; }

        public RepeatingStateReadOnlyException(int stateId) : base(
            "Cannot modify the specified repeating state because it has already ended.")
        {
            this.RepeatingStateId = stateId;
        }
    }
}
