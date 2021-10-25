// CategoryNotFoundException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when the requested board game category is not found.
    /// </summary>
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException() : base("The specified category does not exist.")
        {
        }
    }
}
