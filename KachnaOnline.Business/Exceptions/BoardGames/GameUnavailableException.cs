using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when one or more board games requested for reservations are not available.
    /// </summary>
    public class GameUnavailableException : Exception
    {
        public int UnavailableBoardGameId { get; }

        public GameUnavailableException(int unavailable) : base("Some games are unavailable")
        {
            this.UnavailableBoardGameId = unavailable;
        }
    }
}
