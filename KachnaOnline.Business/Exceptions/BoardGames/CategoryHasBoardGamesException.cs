// CategoryHasBoardGamesException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    /// <summary>
    /// Thrown when a category requested for deletion has board games in it.
    /// </summary>
    public class CategoryHasBoardGamesException : Exception
    {
        public int[] ConflictingGameIds { get; }

        public CategoryHasBoardGamesException(int[] conflicting) : base(
            "Category cannot be deleted because it has linked games.")
        {
            this.ConflictingGameIds = conflicting;
        }
    }
}
