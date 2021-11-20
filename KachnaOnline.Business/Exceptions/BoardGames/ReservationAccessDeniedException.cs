// ReservationAccessDeniedException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    public class ReservationAccessDeniedException : Exception
    {
        /// <summary>
        /// Thrown when a user attempts to modify/get reservation which he does not own.
        /// </summary>
        public ReservationAccessDeniedException() : base("Reservation owned by another user")
        {
        }
    }
}
