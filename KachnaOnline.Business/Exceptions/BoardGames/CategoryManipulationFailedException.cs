using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when board game category modification (e.g. creation) fails (e.g. database error).
    /// </summary>
    public class CategoryManipulationFailedException : Exception
    {
        public CategoryManipulationFailedException() : base("Operation with the given category failed.")
        {
        }
    }
}
