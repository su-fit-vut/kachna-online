// InvalidTransitionException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when the state transition for a reservation is invalid.
    /// </summary>
    public class InvalidTransitionException : Exception
    {
        public InvalidTransitionException() : base("Invalid reservation state transition")
        {
        }
    }
}
