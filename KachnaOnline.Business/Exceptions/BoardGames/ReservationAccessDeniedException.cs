// ReservationAccessDeniedException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    public class ReservationAccessDeniedException : Exception
    {
        public ReservationAccessDeniedException() : base("Reservation owned by another user")
        {
        }
    }
}
