// CategoryHasBoardGamesException.cs
// Author: František Nečas

using System;
using System.Collections.Generic;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    public class CategoryHasBoardGamesException : Exception
    {
        public Array ConflictingGameIds { get; }

        public CategoryHasBoardGamesException(Array conflicting) : base(
            "Category cannot be deleted because it has linked games.")
        {
            ConflictingGameIds = conflicting;
        }
    }
}
