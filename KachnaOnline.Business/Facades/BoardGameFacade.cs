// BoardGameFacade.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Models.BoardGames;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.BoardGames;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.BoardGames;

namespace KachnaOnline.Business.Facades
{
    public class BoardGameFacade
    {
        private readonly IMapper _mapper;
        private readonly IBoardGameService _boardGameService;

        public BoardGameFacade(IBoardGameService boardGameService, IMapper mapper)
        {
            _boardGameService = boardGameService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all categories of board games.
        /// </summary>
        /// <returns>A list of <see cref="CategoryDto"/>.</returns>
        public async Task<List<CategoryDto>> GetCategories()
        {
            var categories = await _boardGameService.GetBoardGameCategories();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        /// <summary>
        /// Returns a category with the given ID.
        /// </summary>
        /// <param name="categoryId">ID of the <see cref="CategoryDto"/> to return.</param>
        /// <returns>A <see cref="CategoryDto"/> with the given ID.</returns>
        /// <exception cref="CategoryNotFoundException">Thrown when a category with the given
        /// <paramref name="categoryId"/> does not exist.</exception>
        public async Task<CategoryDto> GetCategory(int categoryId)
        {
            return _mapper.Map<CategoryDto>(await _boardGameService.GetCategory(categoryId));
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="category"><see cref="CategoryDto"/> to create.</param>
        /// <returns>The created <see cref="CategoryDto"/> with a filled ID.</returns>
        /// <exception cref="CategoryManipulationFailedException">Thrown when the category cannot be created.
        /// This can be caused by a database error.</exception>
        public async Task<CategoryDto> CreateCategory( CategoryDto category)
        {
            var createdCategory = await _boardGameService.CreateCategory(_mapper.Map<Category>(category));
            return _mapper.Map<CategoryDto>(createdCategory);
        }

        /// <summary>
        /// Updates a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to update.</param>
        /// <param name="category"><see cref="CategoryDto"/> representing the new state.</param>
        /// <exception cref="CategoryNotFoundException">When a category with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="CategoryManipulationFailedException">When the category cannot be updated.</exception>
        public async Task UpdateCategory( int id, CategoryDto category)
        {
            await _boardGameService.UpdateCategory(id, _mapper.Map<Category>(category));
        }

        /// <summary>
        /// Deletes a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to delete.</param>
        /// <exception cref="CategoryNotFoundException">When a category with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="CategoryManipulationFailedException">When the category cannot be deleted.</exception>
        /// <exception cref="CategoryHasBoardGamesException">When the category has linked board games.</exception>
        public async Task DeleteCategory(int id)
        {
            await _boardGameService.DeleteCategory(id);
        }

        /// <summary>
        /// Sets <see cref="BoardGameDto"/> attributes to null if the given user does not have the right to see them.
        /// </summary>
        /// <param name="user">User to check the authorization for. All data is visible to board game manager.</param>
        /// <param name="game">Game to hide details of.</param>
        private void HidePrivateBoardGameAttributes(ClaimsPrincipal user, BoardGameDto game)
        {
            if (!user.IsInRole(RoleConstants.BoardGamesManager))
            {
                game.NoteInternal = null;
                game.OwnerId = null;
                game.DefaultReservationDays = null;
                game.Visible = null;
                game.InStock = null;
                game.Unavailable = null;
            }
        }

        /// <summary>
        /// Returns the list of board games.
        /// </summary>
        /// <param name="user">User requesting the data.</param>
        /// <param name="categoryId">If set, returns only the board games of this category ID.</param>
        /// <param name="players">If set, returns only board games which can be played by this many players.</param>
        /// <param name="available">If set, returns only the board games of this availability.</param>
        /// <param name="visible">If set, returns only the boards games with this visibility. It is implicitly
        /// set to true for unauthenticated or regular users.</param>
        /// <returns>A list of <see cref="BoardGameDto"/>. The attributes of each <see cref="BoardGameDto"/>
        /// depend on the user requesting the data.</returns>
        public async Task<List<BoardGameDto>> GetBoardGames(
            ClaimsPrincipal user,
            int? categoryId,
            int? players,
            bool? available,
            bool? visible)
        {
            // Regular users can only see visible games
            if (!user.IsInRole(RoleConstants.BoardGamesManager))
            {
                visible = true;
            }

            var games = await _boardGameService.GetBoardGames(categoryId, players, available, visible);
            var result = new List<BoardGameDto>();
            foreach (var game in games)
            {
                var dto = _mapper.Map<BoardGameDto>(game);
                HidePrivateBoardGameAttributes(user, dto);
                result.Add(dto);
            }

            return result;
        }

        /// <summary>
        /// Returns a board game with the given ID.
        /// </summary>
        /// <param name="user">User requesting the data.</param>
        /// <param name="boardGameId">ID of the <see cref="BoardGameDto"/> to return.</param>
        /// <returns>A <see cref="BoardGameDto"/> with the given ID.</returns>
        /// <exception cref="BoardGameNotFoundException">Thrown when a board game with the given
        /// <paramref name="boardGameId"/> does not exist.</exception>
        public async Task<BoardGameDto> GetBoardGame(ClaimsPrincipal user, int boardGameId)
        {
            var dto = _mapper.Map<BoardGameDto>(await _boardGameService.GetBoardGame(boardGameId));
            HidePrivateBoardGameAttributes(user, dto);
            return dto;
        }

        /// <summary>
        /// Creates a new board game.
        /// </summary>
        /// <param name="game"><see cref="BoardGameDto"/> to create.</param>
        /// <returns>The created <see cref="BoardGameDto"/> with a filled ID.</returns>
        /// <exception cref="BoardGameManipulationFailedException">Thrown when the board game cannot be created.
        /// This can be caused by a database error.</exception>
        /// <exception cref="CategoryNotFoundException">When a category with the ID assigned to the game
        /// does not exist.</exception>
        /// <exception cref="UserNotFoundException">When a user with the ID assigned to the game does
        /// not exist.</exception>
        public async Task<BoardGameDto> CreateBoardGame(BoardGameDto game)
        {
            var createdGame = await _boardGameService.CreateBoardGame(_mapper.Map<BoardGame>(game));
            return _mapper.Map<BoardGameDto>(createdGame);
        }

        /// <summary>
        /// Updates a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="game"><see cref="BoardGameDto"/> representing the new state.</param>
        /// <exception cref="BoardGameNotFoundException">When a board game with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="BoardGameManipulationFailedException">When the board game cannot be updated.</exception>
        /// <exception cref="CategoryNotFoundException">When a category with the ID assigned to the game
        /// does not exist.</exception>
        /// <exception cref="UserNotFoundException">When a user with the ID assigned to the game does
        /// not exist.</exception>
        public async Task UpdateBoardGame(int id, BoardGameDto game)
        {
            await _boardGameService.UpdateBoardGame(id, _mapper.Map<BoardGame>(game));
        }

        /// <summary>
        /// Updates stock of a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="stock"><see cref="BoardGameStockDto"/> representing the new stock state.</param>
        /// <exception cref="BoardGameNotFoundException">When a board game with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="BoardGameManipulationFailedException">When the board game cannot be updated.</exception>
        public async Task UpdateBoardGameStock(int id, BoardGameStockDto stock)
        {
            await _boardGameService.UpdateBoardGame(id, stock.InStock, stock.Unavailable, stock.Visible);
        }
    }
}
