// BoardGameService.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Models.BoardGames;
using KachnaOnline.Business.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.Business.Services
{
    public class BoardGameService : IBoardGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BoardGameService> _logger;
        private readonly IBoardGamesRepository _boardGamesRepository;
        private readonly IBoardGamesCategoryRepository _boardGamesCategoryRepository;
        private readonly IUserRepository _userRepository;

        public BoardGameService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BoardGameService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _boardGamesRepository = _unitOfWork.BoardGameses;
            _boardGamesCategoryRepository = _unitOfWork.BoardGamesCategories;
            _userRepository = _unitOfWork.Users;
        }

        /// <summary>
        /// Calculates availability of the game.
        /// </summary>
        /// <remarks>
        /// Availability of a game is determined based on its stock, number of unavailable games and the number
        /// of reservations.
        /// </remarks>
        /// <param name="game">Game to calculate the availability of.</param>
        private async Task CalculateGameAvailability(BoardGame game)
        {
            // TODO: reservations (repository will be queried, hence the async which doesn't have an effect now)
            game.Available = game.InStock - game.Unavailable;
        }

        /// <inheritdoc />
        public async Task<ICollection<BoardGame>> GetBoardGames(
            int? category,
            int? players,
            bool? available,
            bool? visible)
        {
            var result = new List<BoardGame>();
            await foreach (var game in _boardGamesRepository.GetFilteredGames(category, players, available, visible))
            {
                var gameModel = _mapper.Map<BoardGame>(game);
                await this.CalculateGameAvailability(gameModel);
                result.Add(gameModel);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<BoardGame> GetBoardGame(int boardGameId)
        {
            var game = await _boardGamesRepository.GetWithCategory(boardGameId);
            if (game is null)
                throw new BoardGameNotFoundException();

            var gameModel = _mapper.Map<BoardGame>(game);
            await this.CalculateGameAvailability(gameModel);
            return gameModel;
        }

        /// <inheritdoc />
        public async Task<BoardGame> CreateBoardGame(BoardGame game)
        {
            if (game is null)
                throw new ArgumentNullException(nameof(game));

            if (await _boardGamesCategoryRepository.Get(game.CategoryId) is null)
                throw new CategoryNotFoundException();

            if (game.OwnerId is not null && await _userRepository.Get(game.OwnerId.Value) is null)
                throw new UserNotFoundException(game.OwnerId.Value);

            var gameEntity = _mapper.Map<KachnaOnline.Data.Entities.BoardGames.BoardGame>(game);
            await _boardGamesRepository.Add(gameEntity);
            try
            {
                await _unitOfWork.SaveChanges();
                var newGame = _mapper.Map<BoardGame>(gameEntity);
                await this.CalculateGameAvailability(newGame);
                return newGame;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot save new game.");
                await _unitOfWork.ClearTrackedChanges();
                throw new BoardGameManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task UpdateBoardGame(int id, BoardGame game)
        {
            if (game is null)
                throw new ArgumentNullException(nameof(game));

            var currentGame = await _boardGamesRepository.Get(id);
            if (currentGame is null)
                throw new BoardGameNotFoundException();

            if (await _boardGamesCategoryRepository.Get(game.CategoryId) is null)
                throw new CategoryNotFoundException();

            if (game.OwnerId is not null && await _userRepository.Get(game.OwnerId.Value) is null)
                throw new UserNotFoundException(game.OwnerId.Value);

            _mapper.Map(game, currentGame);
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update a board game.");
                await _unitOfWork.ClearTrackedChanges();
                throw new BoardGameManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task UpdateBoardGame(int id, int newStock, int newUnavailable, bool newVisibility)
        {
            var currentGame = await _boardGamesRepository.Get(id);
            if (currentGame is null)
                throw new BoardGameNotFoundException();

            currentGame.InStock = newStock;
            currentGame.Unavailable = newUnavailable;
            currentGame.Visible = newVisibility;
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update a board game.");
                await _unitOfWork.ClearTrackedChanges();
                throw new BoardGameManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<Category>> GetBoardGameCategories()
        {
            var categories = await _boardGamesCategoryRepository.All().ToListAsync();
            return _mapper.Map<List<Category>>(categories);
        }

        /// <inheritdoc />
        public async Task<Category> GetCategory(int categoryId)
        {
            var category = await _boardGamesCategoryRepository.Get(categoryId);
            if (category is null)
                throw new CategoryNotFoundException();

            return _mapper.Map<Category>(category);
        }

        /// <inheritdoc />
        public async Task<Category> CreateCategory(Category category)
        {
            if (category is null)
                throw new ArgumentNullException(nameof(category));

            var categoryEntity = _mapper.Map<KachnaOnline.Data.Entities.BoardGames.Category>(category);
            await _boardGamesCategoryRepository.Add(categoryEntity);
            try
            {
                await _unitOfWork.SaveChanges();
                return _mapper.Map<Category>(categoryEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot save new category.");
                await _unitOfWork.ClearTrackedChanges();
                throw new CategoryManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task UpdateCategory(int id, Category category)
        {
            if (category is null)
                throw new ArgumentNullException(nameof(category));

            var currentCategory = await _boardGamesCategoryRepository.Get(id);
            if (currentCategory is null)
                throw new CategoryNotFoundException();

            currentCategory.Name = category.Name;
            currentCategory.ColourHex = category.ColourHex;
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update a category.");
                await _unitOfWork.ClearTrackedChanges();
                throw new CategoryManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task DeleteCategory(int id)
        {
            var entity = await _boardGamesCategoryRepository.GetWithBoardGames(id);
            if (entity is null)
                throw new CategoryNotFoundException();

            if (entity.Games.Count != 0)
                throw new CategoryHasBoardGamesException(entity.Games.Select(d => d.Id).ToArray());

            await _boardGamesCategoryRepository.Delete(entity);
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot delete a category.");
                await _unitOfWork.ClearTrackedChanges();
                throw new CategoryManipulationFailedException();
            }
        }
    }
}
