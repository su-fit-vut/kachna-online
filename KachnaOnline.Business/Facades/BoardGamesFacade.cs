// BoardGamesFacade.cs
// Author: František Nečas

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
    public class BoardGamesFacade
    {
        private readonly IMapper _mapper;
        private readonly IBoardGamesService _boardGamesService;

        public BoardGamesFacade(IBoardGamesService boardGamesService, IMapper mapper)
        {
            _boardGamesService = boardGamesService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all categories of board games.
        /// </summary>
        /// <returns>A list of <see cref="CategoryDto"/>.</returns>
        public async Task<IEnumerable<CategoryDto>> GetCategories()
        {
            var categories = await _boardGamesService.GetBoardGameCategories();
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
            return _mapper.Map<CategoryDto>(await _boardGamesService.GetCategory(categoryId));
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="category"><see cref="CreateCategoryDto"/> to create.</param>
        /// <returns>The created <see cref="CategoryDto"/> with a filled ID.</returns>
        /// <exception cref="CategoryManipulationFailedException">Thrown when the category cannot be created.
        /// This can be caused by a database error.</exception>
        public async Task<CategoryDto> CreateCategory(CreateCategoryDto category)
        {
            var createdCategory = await _boardGamesService.CreateCategory(_mapper.Map<Category>(category));
            return _mapper.Map<CategoryDto>(createdCategory);
        }

        /// <summary>
        /// Updates a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to update.</param>
        /// <param name="category"><see cref="CreateCategoryDto"/> representing the new state.</param>
        /// <exception cref="CategoryNotFoundException">When a category with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="CategoryManipulationFailedException">When the category cannot be updated.</exception>
        public async Task UpdateCategory(int id, CreateCategoryDto category)
        {
            await _boardGamesService.UpdateCategory(id, _mapper.Map<Category>(category));
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
            await _boardGamesService.DeleteCategory(id);
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
        /// <returns>A list of <see cref="BoardGameDto"/>. If the <paramref name="user"/> is an authorized
        /// board games manager, returns a list of <see cref="ManagerBoardGameDto"/> instead.</returns>
        public async Task<IEnumerable<BoardGameDto>> GetBoardGames(
            ClaimsPrincipal user,
            int? categoryId,
            int? players,
            bool? available,
            bool? visible)
        {
            // Regular users can only see visible games
            if (!user.IsInRole(RoleConstants.BoardGamesManager))
                visible = true;

            var games = await _boardGamesService.GetBoardGames(categoryId, players, available, visible);
            if (user.IsInRole(RoleConstants.BoardGamesManager))
            {
                return _mapper.Map<List<ManagerBoardGameDto>>(games);
            }

            return _mapper.Map<List<BoardGameDto>>(games);
        }

        /// <summary>
        /// Returns a board game with the given ID.
        /// </summary>
        /// <param name="user">User requesting the data.</param>
        /// <param name="boardGameId">ID of the <see cref="BoardGameDto"/> to return.</param>
        /// <returns>A <see cref="BoardGameDto"/> with the given ID. If the <paramref name="user"/> is an authorized
        /// board games manager, returns <see cref="ManagerBoardGameDto"/> instead.</returns>
        /// <exception cref="BoardGameNotFoundException">Thrown when a board game with the given
        /// <paramref name="boardGameId"/> does not exist.</exception>
        /// <exception cref="NotAuthenticatedException">Thrown when a board game with the given
        /// <paramref name="boardGameId"/> exists but is invisible and the <paramref name="user"/>
        /// is not authenticated.</exception>
        /// <exception cref="NotABoardGamesManagerException">Thrown when a board game with the given
        /// <paramref name="boardGameId"/> exists but is invisible and the <paramref name="user"/>
        /// is not a board games manager.</exception>
        public async Task<BoardGameDto> GetBoardGame(ClaimsPrincipal user, int boardGameId)
        {
            var game = await _boardGamesService.GetBoardGame(boardGameId);
            if (!game.Visible)
            {
                if (!(user.Identity?.IsAuthenticated ?? false))
                {
                    throw new NotAuthenticatedException();
                }

                if (!user.IsInRole(RoleConstants.BoardGamesManager))
                {
                    throw new NotABoardGamesManagerException();
                }
            }

            if (user.IsInRole(RoleConstants.BoardGamesManager))
            {
                return _mapper.Map<ManagerBoardGameDto>(game);
            }

            return _mapper.Map<BoardGameDto>(game);
        }

        /// <summary>
        /// Creates a new board game.
        /// </summary>
        /// <param name="game"><see cref="CreateBoardGameDto"/> to create.</param>
        /// <returns>The created <see cref="ManagerBoardGameDto"/> with a filled ID.</returns>
        /// <exception cref="BoardGameManipulationFailedException">Thrown when the board game cannot be created.
        /// This can be caused by a database error.</exception>
        /// <exception cref="CategoryNotFoundException">When a category with the ID assigned to the game
        /// does not exist.</exception>
        /// <exception cref="UserNotFoundException">When a user with the ID assigned to the game does
        /// not exist.</exception>
        public async Task<BoardGameDto> CreateBoardGame(CreateBoardGameDto game)
        {
            var createdGame = await _boardGamesService.CreateBoardGame(_mapper.Map<BoardGame>(game));
            return _mapper.Map<ManagerBoardGameDto>(createdGame);
        }

        /// <summary>
        /// Updates a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="game"><see cref="CreateBoardGameDto"/> representing the new state.</param>
        /// <exception cref="BoardGameNotFoundException">When a board game with the given <paramref name="id"/> does not
        /// exist.</exception>
        /// <exception cref="BoardGameManipulationFailedException">When the board game cannot be updated.</exception>
        /// <exception cref="CategoryNotFoundException">When a category with the ID assigned to the game
        /// does not exist.</exception>
        /// <exception cref="UserNotFoundException">When a user with the ID assigned to the game does
        /// not exist.</exception>
        public async Task UpdateBoardGame(int id, CreateBoardGameDto game)
        {
            await _boardGamesService.UpdateBoardGame(id, _mapper.Map<BoardGame>(game));
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
            await _boardGamesService.UpdateBoardGame(id, stock.InStock, stock.Unavailable, stock.Visible);
        }
    }
}
