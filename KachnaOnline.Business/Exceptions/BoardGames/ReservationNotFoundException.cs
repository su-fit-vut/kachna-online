// ReservationNotFoundException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when the requested board games reservation does not exist.
    /// </summary>
    public class ReservationNotFoundException : Exception
    {
        public ReservationNotFoundException() : base("No such reservation exists.")
        {
        }
    }
}
