// IBoardGameService.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Models.BoardGames;

namespace KachnaOnline.Business.Services.Abstractions
{
    public interface IBoardGameService
    {
        /// <summary>
        /// Returns a list of board games in a category with the given ID.
        /// </summary>
        /// <param name="categoryId">ID of the category to search games in.</param>
        /// <returns>A list of <see cref="BoardGame"/>.</returns>
        Task<ICollection<BoardGame>> GetBoardGamesInCategory(int categoryId);
        /// <summary>
        /// Returns a list of all board games.
        /// </summary>
        /// <returns>A list of <see cref="BoardGame"/>.</returns>
        Task<ICollection<BoardGame>> GetBoardGames();

        /// <summary>
        /// Returns a list of all game categories.
        /// </summary>
        /// <returns>A List of <see cref="Category">categories</see>.</returns>
        Task<ICollection<Category>> GetBoardGameCategories();
    }
}
