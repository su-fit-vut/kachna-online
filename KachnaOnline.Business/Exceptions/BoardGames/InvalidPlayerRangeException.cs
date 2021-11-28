// InvalidPlayerRangeException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when the added/updated board game has an invalid range of players.
    /// </summary>
    public class InvalidPlayerRangeException : Exception
    {
        public InvalidPlayerRangeException() : base("Minimum players must be less or equal than maximum players.")
        {
        }
    }
}
