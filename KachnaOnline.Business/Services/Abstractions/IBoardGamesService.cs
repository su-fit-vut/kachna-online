// IBoardGamesService.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Models.BoardGames;

namespace KachnaOnline.Business.Services.Abstractions
{
    public interface IBoardGamesService
    {
        /// <summary>
        /// Returns a list of all board games.
        /// </summary>
        /// <param name="category">If given, board games only from this category should be returned.</param>
        /// <param name="players">If given, only board games which can be played by this number of players
        /// will be returned.</param>
        /// <param name="available">If given, board games only with this availability should be returned.</param>
        /// <param name="visible">If given, board games only with this visibility should be returned.</param>
        /// <returns>A list of <see cref="BoardGame"/> filtered based on the given params.</returns>
        Task<ICollection<BoardGame>> GetBoardGames(int? category, int? players, bool? available, bool? visible);

        /// <summary>
        /// Returns a board game with the given ID.
        /// </summary>
        /// <param name="boardGameId">ID of the <see cref="BoardGame"/> to return.</param>
        /// <returns>A <see cref="BoardGame"/> corresponding to the given ID.</returns>
        /// <exception cref="BoardGameNotFoundException">Thrown when a board game with the given
        /// <paramref name="boardGameId"/> does not exist.</exception>
        Task<BoardGame> GetBoardGame(int boardGameId);

        /// <summary>
        /// Creates a new board game.
        /// </summary>
        /// <param name="game"><see cref="BoardGame"/> to create.</param>
        /// <returns>The created <see cref="BoardGame"/> with a filled ID.</returns>
        /// <exception cref="BoardGameManipulationFailedException">Thrown when the board game cannot be created.
        /// This can be caused by a database error.</exception>
        /// <exception cref="CategoryNotFoundException">When a category with the ID assigned to the game
        /// does not exist.</exception>
        /// <exception cref="UserNotFoundException">When a user with the ID assigned to the game does
        /// not exist.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="game"/> is null.</exception>
        Task<BoardGame> CreateBoardGame(BoardGame game);

        /// <summary>
        /// Updates a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="game"><see cref="BoardGame"/> representing the new state.</param>
        /// <exception cref="BoardGameNotFoundException">When a board game with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="BoardGameManipulationFailedException">When the board game cannot be updated.</exception>
        /// <exception cref="CategoryNotFoundException">When a category with the ID assigned to the game
        /// does not exist.</exception>
        /// <exception cref="UserNotFoundException">When a user with the ID assigned to the game does
        /// not exist.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="game"/> is null.</exception>
        Task UpdateBoardGame(int id, BoardGame game);

        /// <summary>
        /// Updates stock of a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="newStock">New number of pieces of the game in stock.</param>
        /// <param name="newUnavailable">New number of unavailable pieces of the game.</param>
        /// <param name="newVisibility">New visibility of the game.</param>
        /// <exception cref="BoardGameNotFoundException">When a board game with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="BoardGameManipulationFailedException">When the board game cannot be updated.</exception>
        Task UpdateBoardGame(int id, int newStock, int newUnavailable, bool newVisibility);

        /// <summary>
        /// Returns a list of all game categories.
        /// </summary>
        /// <returns>A List of <see cref="Category">categories</see>.</returns>
        Task<ICollection<Category>> GetBoardGameCategories();

        /// <summary>
        /// Returns a category with the given ID.
        /// </summary>
        /// <param name="categoryId">ID of the <see cref="Category"/> to return.</param>
        /// <returns>A <see cref="Category"/> corresponding to the given ID.</returns>
        /// <exception cref="CategoryNotFoundException">Thrown when a category with the given
        /// <paramref name="categoryId"/> does not exist.</exception>
        Task<Category> GetCategory(int categoryId);

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="category"><see cref="Category"/> to create.</param>
        /// <returns>The created <see cref="Category"/> with a filled ID.</returns>
        /// <exception cref="CategoryManipulationFailedException">Thrown when the category cannot be created.
        /// This can be caused by a database error.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="category"/> is null.</exception>
        Task<Category> CreateCategory(Category category);

        /// <summary>
        /// Updates a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to update.</param>
        /// <param name="category"><see cref="Category"/> representing the new state.</param>
        /// <exception cref="CategoryNotFoundException">When a category with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="CategoryManipulationFailedException">When the category cannot be updated.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="category"/> is null.</exception>
        Task UpdateCategory(int id, Category category);

        /// <summary>
        /// Deletes a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to delete.</param>
        /// <exception cref="CategoryNotFoundException">When a category with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="CategoryManipulationFailedException">When the category cannot be deleted.</exception>
        /// <exception cref="CategoryHasBoardGamesException">When the category has linked board games. Only
        /// the property of conflicting <see cref="BoardGame"/> is filled by the service.</exception>
        Task DeleteCategory(int id);

        /// <summary>
        /// Creates a new reservation.
        /// </summary>
        /// <param name="reservation"><see cref="Reservation"/> to create.</param>
        /// <param name="reservationGames">Array of game IDs to reserve.</param>
        /// <returns>The created <see cref="Reservation"/> with a filled ID.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="reservation"/> is null.</exception>
        /// <exception cref="GameUnavailableException">When some of the requested board games are not
        /// available.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be created.</exception>
        Task<Reservation> CreateReservation(Reservation reservation, int[] reservationGames);
    }
}
