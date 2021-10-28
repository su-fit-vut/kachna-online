// NotABoardGamesManagerException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when a regular user requests an operation that can only be done by a board game manager.
    /// </summary>
    public class NotABoardGamesManagerException : Exception
    {
        public NotABoardGamesManagerException() : base("You must be a board game manager to do that")
        {
        }
    }
}
