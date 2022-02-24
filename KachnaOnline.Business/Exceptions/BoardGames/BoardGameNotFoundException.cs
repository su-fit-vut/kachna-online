using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when the requested board game does not exist.
    /// </summary>
    public class BoardGameNotFoundException : Exception
    {
        public BoardGameNotFoundException() : base("No such board game exists.")
        {
        }
    }
}
