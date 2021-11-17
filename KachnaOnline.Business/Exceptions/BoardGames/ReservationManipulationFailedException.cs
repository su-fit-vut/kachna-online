// ReservationManipulationFailedException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when reservation manipulation (e.g. creation) fails. This could be caused by a database error.
    /// </summary>
    public class ReservationManipulationFailedException : Exception
    {
        public ReservationManipulationFailedException() : base("Operation with the given reservation failed.")
        {
        }
    }
}
