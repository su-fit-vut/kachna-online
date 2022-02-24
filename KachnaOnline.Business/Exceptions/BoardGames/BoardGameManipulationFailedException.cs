using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when a manipulation (e.g. creation) with a board game fails (e.g. due to a database error).
    /// </summary>
    public class BoardGameManipulationFailedException : Exception
    {
        public BoardGameManipulationFailedException() : base("Operation with the given board game failed.")
        {
        }
    }
}
