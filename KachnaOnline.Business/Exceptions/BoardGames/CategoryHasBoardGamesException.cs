// CategoryHasBoardGamesException.cs
// Author: František Nečas

using System;
using System.Collections.Generic;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
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
