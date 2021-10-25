// CategoryHasBoardGamesException.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using KachnaOnline.Business.Models.BoardGames;
using KachnaOnline.Dto.BoardGames;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    public class CategoryHasBoardGamesException : Exception
    {
        public List<BoardGame> ConflictingGames { get; set; }
        public List<BoardGameDto> ConflictingGamesDto { get; set; }

        public CategoryHasBoardGamesException(List<BoardGame> conflicting) : base(
            "Category cannot be deleted because it has linked games")
        {
            ConflictingGames = conflicting;
        }

    }
}
