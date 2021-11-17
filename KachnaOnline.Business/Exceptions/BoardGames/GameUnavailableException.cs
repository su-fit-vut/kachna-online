// GameUnavailableException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when one or more board games requested for reservations are not available.
    /// </summary>
    public class GameUnavailableException : Exception
    {
        public int[] UnavailableBoardGameIds { get; }

        public GameUnavailableException(int[] unavailable) : base("Some games are unavailable")
        {
            UnavailableBoardGameIds = unavailable;
        }
    }
}
