// StateNotFoundException.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Exceptions.ClubStates
{
    public class RepeatingStateNotFoundException : Exception
    {
        public int? RepeatingStateId { get; }

        public RepeatingStateNotFoundException() : base("The specified repeating state does not exist.")
        {
        }

        public RepeatingStateNotFoundException(int repeatingStateId) : base($"No repeating state with ID {repeatingStateId} exists.")
        {
            this.RepeatingStateId = repeatingStateId;
        }

        public RepeatingStateNotFoundException(string message) : base(message)
        {
        }
    }
}
