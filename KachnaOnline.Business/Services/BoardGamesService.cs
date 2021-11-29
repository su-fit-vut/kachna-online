// BoardGamesService.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Models.BoardGames;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Business.Services.BoardGamesNotifications.Abstractions;
using KachnaOnline.Business.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KachnaOnline.Business.Services
{
    public class BoardGamesService : IBoardGamesService
    {
        private readonly IOptionsMonitor<BoardGamesOptions> _boardGamesOptionsMonitor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BoardGamesService> _logger;
        private readonly IBoardGamesRepository _boardGamesRepository;
        private readonly IBoardGameCategoriesRepository _boardGamesCategoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IReservationItemRepository _reservationItemRepository;
        private readonly IReservationItemEventRepository _reservationItemEventRepository;
        private readonly IServiceProvider _serviceProvider;

        public BoardGamesService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BoardGamesService> logger,
            IOptionsMonitor<BoardGamesOptions> boardGamesOptionsMonitor, IServiceProvider serviceProvider)
        {
            _boardGamesOptionsMonitor = boardGamesOptionsMonitor;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _boardGamesRepository = _unitOfWork.BoardGames;
            _boardGamesCategoryRepository = _unitOfWork.BoardGamesCategories;
            _userRepository = _unitOfWork.Users;
            _reservationRepository = _unitOfWork.Reservations;
            _reservationItemRepository = _unitOfWork.ReservationItems;
            _reservationItemEventRepository = _unitOfWork.ReservationItemEvents;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Calculates availability of the game.
        /// </summary>
        /// <remarks>
        /// Availability of a game is determined based on its stock, number of unavailable games and the number
        /// of reservations.
        /// </remarks>
        /// <param name="game">Game to calculate the availability of.</param>
        private void CalculateGameAvailability(BoardGame game)
        {
            var reserved = _reservationItemRepository.CountCurrentlyReservingGame(game.Id);
            game.Available = game.InStock - game.Unavailable - reserved;
        }

        /// <inheritdoc />
        public async Task<ICollection<BoardGame>> GetBoardGames(
            int? category,
            int? players,
            bool? available,
            bool? visible)
        {
            var result = new List<BoardGame>();
            foreach (var game in await _boardGamesRepository.GetFilteredGames(category, players, available, visible))
            {
                var gameModel = _mapper.Map<BoardGame>(game);
                this.CalculateGameAvailability(gameModel);
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
            this.CalculateGameAvailability(gameModel);
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

            if (game.PlayersMin.HasValue && game.PlayersMax.HasValue && game.PlayersMin.Value > game.PlayersMax.Value)
                throw new InvalidPlayerRangeException();

            var gameEntity = _mapper.Map<KachnaOnline.Data.Entities.BoardGames.BoardGame>(game);
            await _boardGamesRepository.Add(gameEntity);
            try
            {
                await _unitOfWork.SaveChanges();
                var newGame = _mapper.Map<BoardGame>(gameEntity);
                this.CalculateGameAvailability(newGame);
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
            game.Id = id;
            if (game is null)
                throw new ArgumentNullException(nameof(game));

            var currentGame = await _boardGamesRepository.Get(id);
            if (currentGame is null)
                throw new BoardGameNotFoundException();

            if (await _boardGamesCategoryRepository.Get(game.CategoryId) is null)
                throw new CategoryNotFoundException();

            if (game.OwnerId is not null && await _userRepository.Get(game.OwnerId.Value) is null)
                throw new UserNotFoundException(game.OwnerId.Value);

            if (game.PlayersMin.HasValue && game.PlayersMax.HasValue && game.PlayersMin.Value > game.PlayersMax.Value)
                throw new InvalidPlayerRangeException();

            // FIXME: Exception is thrown when update is performed on a board game with reservations due to FK
            //      constraints violations. Temporary workaround
            // _mapper.Map(game, currentGame);
            currentGame.Name = game.Name;
            currentGame.Description = game.Description;
            currentGame.ImageUrl = game.ImageUrl;
            currentGame.PlayersMin = game.PlayersMin;
            currentGame.PlayersMax = game.PlayersMax;
            currentGame.CategoryId = game.CategoryId;
            currentGame.OwnerId = game.OwnerId;
            currentGame.NoteInternal = game.NoteInternal;
            currentGame.InStock = game.InStock;
            currentGame.Unavailable = game.Unavailable;
            currentGame.Visible = game.Visible;
            currentGame.DefaultReservationTime = game.DefaultReservationTime;
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

        /// <summary>
        /// Updates the state of a single reservation item based on events.
        /// </summary>
        /// <param name="item">Reservation item to update.</param>
        private async Task UpdateReservationItemState(ReservationItem item)
        {
            var now = DateTime.Now;
            if ((item.ExpiresOn ?? now) < now)
            {
                item.State = ReservationItemState.Expired;
            }
            else
            {
                var lastEvent =
                    _mapper.Map<ReservationItemEvent>(await _reservationItemEventRepository.GetLatestEvent(item.Id));
                if (lastEvent is not null)
                {
                    item.State = lastEvent.NewState;
                }
            }
        }

        /// <summary>
        /// Checks if a reservation is in a given state.
        /// </summary>
        /// <param name="reservationId">ID of a reservation to check.</param>
        /// <param name="state">Requested state.</param>
        /// <returns>Whether the reservation is in the given state.</returns>
        private async Task<bool> IsReservationInState(int reservationId, ReservationState? state)
        {
            if (state is null)
                return true;

            var items = new List<ReservationItem>();
            foreach (var item in await _reservationItemRepository.GetItemsInReservation(reservationId))
            {
                var itemModel = _mapper.Map<ReservationItem>(item);
                await this.UpdateReservationItemState(itemModel);
                items.Add(itemModel);
            }

            return state switch
            {
                ReservationState.New => items.Any(i => i.State == ReservationItemState.New),
                ReservationState.Expired => items.Any(i => i.State == ReservationItemState.Expired),
                ReservationState.Done => items.All(i =>
                    i.State is ReservationItemState.Done or ReservationItemState.Cancelled),
                ReservationState.Current => items.Any(i =>
                    i.State is not ReservationItemState.Done and not ReservationItemState.Cancelled),
                _ => false
            };
        }

        /// <inheritdoc />
        public async Task<ICollection<Reservation>> GetUserReservations(int user, ReservationState? state)
        {
            var reservations = new List<Reservation>();
            foreach (var reservation in await _reservationRepository.GetByUserId(user))
            {
                if (!state.HasValue || await this.IsReservationInState(reservation.Id, state.Value))
                {
                    reservations.Add(_mapper.Map<Reservation>(reservation));
                }
            }

            return reservations;
        }

        /// <inheritdoc />
        public async Task<ICollection<Reservation>> GetAllReservations(ReservationState? state, int? assignedTo)
        {
            var reservations = new List<Reservation>();
            foreach (var reservation in await _reservationRepository.GetByAssignedUserId(assignedTo))
            {
                if (!state.HasValue || await this.IsReservationInState(reservation.Id, state.Value))
                {
                    reservations.Add(_mapper.Map<Reservation>(reservation));
                }
            }

            return reservations;
        }

        /// <inheritdoc />
        public async Task<ICollection<ReservationItem>> GetReservationItems(int reservationId)
        {
            var items = new List<ReservationItem>();
            foreach (var item in await _reservationItemRepository.GetItemsInReservation(reservationId))
            {
                var itemModel = _mapper.Map<ReservationItem>(item);
                await this.UpdateReservationItemState(itemModel);
                items.Add(itemModel);
            }

            return items;
        }

        /// <inheritdoc />
        public async Task<ReservationItem> GetReservationItem(int itemId)
        {
            var item = await _reservationItemRepository.Get(itemId);
            if (item is null)
                return null;

            var itemModel = _mapper.Map<ReservationItem>(item);
            await this.UpdateReservationItemState(itemModel);
            return itemModel;
        }

        /// <inheritdoc />
        public async Task<int?> GetReservationItemAssignee(int itemId)
        {
            return await _reservationItemEventRepository.GetAssignee(itemId);
        }

        /// <inheritdoc />
        public async Task<Reservation> GetReservation(int reservationId)
        {
            var reservation = await _reservationRepository.Get(reservationId);
            if (reservation is null)
                throw new ReservationNotFoundException();

            return _mapper.Map<Reservation>(reservation);
        }

        /// <summary>
        /// Creates new reservation items.
        /// </summary>
        /// <remarks>
        /// Assumes that a transaction on <see cref="IUnitOfWork"/> is in progress.
        ///
        /// Also creates a <see cref="ReservationItemEvent"/> of type Created.
        /// </remarks>
        /// <param name="reservationId">ID which the items will belong to.</param>
        /// <param name="createdBy">ID of the user requesting the creation.</param>
        /// <param name="reservationGames">Array of game IDs to reserve.</param>
        /// <exception cref="BoardGameNotFoundException">When a requested game does not exist.</exception>
        /// <exception cref="GameUnavailableException">When a requested game is not available.</exception>
        private async Task CreateReservationItems(int reservationId, int createdBy, IEnumerable<int> reservationGames)
        {
            foreach (var gameId in reservationGames)
            {
                var game = await _boardGamesRepository.Get(gameId);
                if (game is null)
                {
                    await _unitOfWork.RollbackTransaction();
                    throw new BoardGameNotFoundException();
                }

                var gameModel = _mapper.Map<BoardGame>(game);
                this.CalculateGameAvailability(gameModel);
                if (gameModel.Available <= 0)
                {
                    await _unitOfWork.RollbackTransaction();
                    throw new GameUnavailableException(gameId);
                }

                var reservationItem = new ReservationItem(reservationId, gameId);
                var itemEntity = _mapper.Map<KachnaOnline.Data.Entities.BoardGames.ReservationItem>(reservationItem);
                await _reservationItemRepository.Add(itemEntity);
                await _unitOfWork.SaveChanges();

                var creationEvent = new ReservationItemEvent(ReservationEventType.Created, itemEntity.Id, createdBy);
                var eventEntity =
                    _mapper.Map<KachnaOnline.Data.Entities.BoardGames.ReservationItemEvent>(creationEvent);
                await _reservationItemEventRepository.Add(eventEntity);
                await _unitOfWork.SaveChanges();
            }
        }

        /// <inheritdoc />
        public async Task<Reservation> CreateReservation(Reservation reservation, int createdBy,
            IEnumerable<int> reservationGames)
        {
            if (reservation is null)
                throw new ArgumentNullException(nameof(reservation));

            if (await _userRepository.Get(reservation.MadeById) is null)
                throw new UserNotFoundException(reservation.MadeById);

            if (await _userRepository.Get(createdBy) is null)
                throw new UserNotFoundException(createdBy);

            try
            {
                await _unitOfWork.BeginTransaction();
                var entity = _mapper.Map<KachnaOnline.Data.Entities.BoardGames.Reservation>(reservation);
                await _reservationRepository.Add(entity);
                await _unitOfWork.SaveChanges();

                var createdReservation = _mapper.Map<Reservation>(entity);
                await this.CreateReservationItems(createdReservation.Id, createdBy, reservationGames);
                await _unitOfWork.CommitTransaction();
                // Only notify for user-created reservation (not created by a manager)
                if (createdReservation.MadeById == createdBy)
                {
                    TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, _) =>
                    {
                        var notificationService = services.GetRequiredService<IBoardGamesNotificationService>();
                        await notificationService.TriggerReservationCreated(createdReservation.Id);
                    });
                }

                return createdReservation;
            }
            // Re-throw exceptions related to game availability
            catch (BoardGameNotFoundException)
            {
                _logger.LogError("Cannot add items to a reservation, rolling changes back.");
                await _unitOfWork.RollbackTransaction();
                throw;
            }
            catch (GameUnavailableException)
            {
                _logger.LogError("Cannot add items to a reservation, rolling changes back.");
                await _unitOfWork.RollbackTransaction();
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot create a reservation, rolling changes back.");
                await _unitOfWork.RollbackTransaction();
                throw new ReservationManipulationFailedException();
            }
        }

        public async Task AddReservationItems(int reservationId, int addedBy, IEnumerable<int> newGames)
        {
            if (await _userRepository.Get(addedBy) is null)
                throw new UserNotFoundException(addedBy);

            var reservation = await _reservationRepository.Get(reservationId);
            if (reservation is null)
                throw new ReservationNotFoundException();

            try
            {
                await _unitOfWork.BeginTransaction();
                await this.CreateReservationItems(reservationId, addedBy, newGames);
                await _unitOfWork.CommitTransaction();
            }
            // Re-throw exceptions related to game availability
            catch (BoardGameNotFoundException)
            {
                _logger.LogError("Cannot add items to a reservation, rolling changes back.");
                await _unitOfWork.RollbackTransaction();
                throw;
            }
            catch (GameUnavailableException)
            {
                _logger.LogError("Cannot add items to a reservation, rolling changes back.");
                await _unitOfWork.RollbackTransaction();
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot add items to a reservation, rolling changes back.");
                await _unitOfWork.RollbackTransaction();
                throw new ReservationManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task UpdateReservationNote(int id, int userId, string note)
        {
            if (note is null)
                throw new ArgumentNullException(nameof(note));

            var reservation = await _reservationRepository.Get(id);
            if (reservation is null)
                throw new ReservationNotFoundException();

            if (reservation.MadeById != userId)
                throw new ReservationAccessDeniedException();

            reservation.NoteUser = note;
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update user note in reservation.");
                await _unitOfWork.ClearTrackedChanges();
                throw new ReservationManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task UpdateReservationNoteInternal(int id, string note)
        {
            if (note is null)
                throw new ArgumentNullException(nameof(note));

            var reservation = await _reservationRepository.Get(id);
            if (reservation is null)
                throw new ReservationNotFoundException();

            reservation.NoteInternal = note;
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update internal note in reservation.");
                await _unitOfWork.ClearTrackedChanges();
                throw new ReservationManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task UpdateReservationDiscordMessageId(int id, ulong messageId)
        {
            var reservation = await _reservationRepository.Get(id);
            if (reservation is null)
                throw new ReservationNotFoundException();

            reservation.WebhookMessageId = messageId;
            try
            {
                await _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update webhook message ID in reservation.");
                await _unitOfWork.ClearTrackedChanges();
                throw new ReservationManipulationFailedException();
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<ReservationItemEvent>> GetItemHistory(int reservationId, int itemId)
        {
            var item = await _reservationItemRepository.Get(itemId);
            if (item is null || item.ReservationId != reservationId)
                throw new ReservationNotFoundException();

            return _mapper.Map<List<ReservationItemEvent>>(
                await _reservationItemEventRepository.GetByItemIdChronologically(itemId));
        }

        /// <inheritdoc />
        public async Task<ReservationItemEvent> GetLatestEvent(int reservationId, int itemId)
        {
            var item = await _reservationItemRepository.Get(itemId);
            if (item is null || item.ReservationId != reservationId)
                throw new ReservationNotFoundException();
            return _mapper.Map<ReservationItemEvent>(await _reservationItemEventRepository.GetLatestEvent(itemId));
        }

        /// <summary>
        /// Handles a simple event transition which requires only a new event to be written to DB.
        /// </summary>
        /// <remarks>
        /// Such events are:
        ///     - Assigned
        ///     - Cancelled
        ///     - ExtensionRequested
        ///     - ExtensionRefused
        ///     - Returned
        /// </remarks>
        /// <param name="userId">ID of the user requesting the change.</param>
        /// <param name="itemId">ID of the item the change ties to.</param>
        /// <param name="eventType">Type of event to insert.</param>
        private async Task HandleSimpleTransition(int userId, int itemId, ReservationEventType eventType)
        {
            var eventModel = new ReservationItemEvent(eventType, itemId, userId);
            await _reservationItemEventRepository.Add(
                _mapper.Map<KachnaOnline.Data.Entities.BoardGames.ReservationItemEvent>(eventModel));
        }

        /// <summary>
        /// Handles an extension granted event.
        /// </summary>
        /// <param name="userId">ID of the user granting the extension.</param>
        /// <param name="item">Item which the change ties to.</param>
        private async Task HandleExtensionGrantedTransition(int userId, ReservationItem item)
        {
            // No way how ExpiresOn is null due to the automaton but make everyone happy and use a default.
            var expiration =
                (item.ExpiresOn ?? DateTime.Now).AddDays(_boardGamesOptionsMonitor.CurrentValue.ExtensionDays);
            var eventModel =
                new ReservationItemEvent(ReservationEventType.ExtensionGranted, item.Id, userId, expiration);
            await _reservationItemEventRepository.Add(
                _mapper.Map<KachnaOnline.Data.Entities.BoardGames.ReservationItemEvent>(eventModel));
            await _reservationItemRepository.UpdateExpiration(item.Id, expiration);
        }

        /// <summary>
        /// Handles a single reservation item state transition.
        /// </summary>
        /// <remarks>
        /// Assumes that a transaction on <see cref="IUnitOfWork"/> is in progress.
        ///
        /// The logic is based on a finite automaton. Its transitions are the following:
        ///     Created -> {Assigned, Cancelled}
        ///     Cancelled -> {}
        ///     Assigned -> {HandedOver}
        ///     HandedOver -> {Returned, ExtensionGranted, ExtensionRequested}
        ///     ExtensionRequested -> {Returned, ExtensionRefused, ExtensionGranted}
        ///     ExtensionGranted -> {Returned, ExtensionRequested, ExtensionGranted}
        ///     ExtensionRefused -> {Returned, ExtensionRequested, ExtensionGranted}
        ///     Returned -> {}
        ///
        /// Assumes that the Created event is implicitly inserted on reservation creation.
        /// </remarks>
        /// <param name="userId">ID of the user requesting the change.</param>
        /// <param name="item">Item which the change ties to.</param>
        /// <param name="newEvent">Type of event to insert.</param>
        /// <exception cref="InvalidTransitionException">When the transition cannot be performed (i.e. the requested
        /// state cannot be reached from the last state).</exception>
        private async Task HandleStateTransition(int userId, ReservationItem item, ReservationEventType newEvent)
        {
            var lastEvent =
                _mapper.Map<ReservationItemEvent>(await _reservationItemEventRepository.GetLatestEvent(item.Id));
            var now = DateTime.Now;
            switch (lastEvent.Type)
            {
                case ReservationEventType.Created:
                    if (newEvent is ReservationEventType.Assigned or ReservationEventType.Cancelled)
                    {
                        await this.HandleSimpleTransition(userId, item.Id, newEvent);
                    }
                    else
                    {
                        throw new InvalidTransitionException();
                    }

                    break;
                case ReservationEventType.Assigned:
                    if (newEvent == ReservationEventType.Cancelled)
                    {
                        await this.HandleSimpleTransition(userId, item.Id, newEvent);
                    }
                    else if (newEvent == ReservationEventType.HandedOver)
                    {
                        var game = await _boardGamesRepository.Get(item.BoardGameId);
                        var expiration = now.Add(game.DefaultReservationTime ??
                                                 TimeSpan.FromDays(_boardGamesOptionsMonitor.CurrentValue
                                                     .DefaultReservationDays));
                        var eventModel = new ReservationItemEvent(newEvent, item.Id, userId, expiration);
                        await _reservationItemEventRepository.Add(
                            _mapper.Map<KachnaOnline.Data.Entities.BoardGames.ReservationItemEvent>(eventModel));
                        await _reservationItemRepository.UpdateExpiration(item.Id, expiration);
                    }
                    else
                    {
                        throw new InvalidTransitionException();
                    }

                    break;
                case ReservationEventType.HandedOver:
                    if (newEvent is ReservationEventType.Returned or ReservationEventType.ExtensionRequested)
                    {
                        await this.HandleSimpleTransition(userId, item.Id, newEvent);
                    }
                    else if (newEvent == ReservationEventType.ExtensionGranted)
                    {
                        await this.HandleExtensionGrantedTransition(userId, item);
                    }
                    else
                    {
                        throw new InvalidTransitionException();
                    }

                    break;
                case ReservationEventType.ExtensionRequested:
                    if (newEvent is ReservationEventType.Returned or ReservationEventType.ExtensionRefused)
                    {
                        await this.HandleSimpleTransition(userId, item.Id, newEvent);
                    }
                    else if (newEvent == ReservationEventType.ExtensionGranted)
                    {
                        await this.HandleExtensionGrantedTransition(userId, item);
                    }
                    else
                    {
                        throw new InvalidTransitionException();
                    }

                    break;
                case ReservationEventType.ExtensionGranted:
                    if (newEvent is ReservationEventType.Returned or ReservationEventType.ExtensionRequested)
                    {
                        await this.HandleSimpleTransition(userId, item.Id, newEvent);
                    }
                    else if (newEvent == ReservationEventType.ExtensionGranted)
                    {
                        await this.HandleExtensionGrantedTransition(userId, item);
                    }
                    else
                    {
                        throw new InvalidTransitionException();
                    }

                    break;
                case ReservationEventType.ExtensionRefused:
                    if (newEvent is ReservationEventType.Returned or ReservationEventType.ExtensionRequested)
                    {
                        await this.HandleSimpleTransition(userId, item.Id, newEvent);
                    }
                    else if (newEvent == ReservationEventType.ExtensionGranted)
                    {
                        await this.HandleExtensionGrantedTransition(userId, item);
                    }
                    else
                    {
                        throw new InvalidTransitionException();
                    }

                    break;
                default:
                    // Cancelled and Returned are finish states
                    throw new InvalidTransitionException();
            }

            await _unitOfWork.SaveChanges();
        }

        /// <inheritdoc />
        public async Task ModifyItemState(ClaimsPrincipal user, int reservationId, int itemId,
            ReservationEventType newEvent)
        {
            var item = await _reservationItemRepository.Get(itemId);
            var reservation = await _reservationRepository.Get(reservationId);
            if (item is null || reservation is null || item.ReservationId != reservationId)
                throw new ReservationNotFoundException();

            // Facade has already blocked manager-only event types from regular users. A request from regular user
            // with malicious intent hence could only be of type ExtensionRequested or Cancelled. Check authorization.
            var userId = int.Parse(user.FindFirstValue(IdentityConstants.IdClaim));
            // Only the user whose reservation this is can request extension.
            if (newEvent == ReservationEventType.ExtensionRequested && userId != reservation.MadeById)
                throw new ReservationAccessDeniedException();

            // Only Cancelled request from a regular user is needed to be checked now. A board games manager
            // can also request cancellation.
            if (!user.IsInRole(AuthConstants.BoardGamesManager) && userId != reservation.MadeById)
                throw new ReservationAccessDeniedException();

            var itemModel = _mapper.Map<ReservationItem>(item);
            try
            {
                await _unitOfWork.BeginTransaction();
                await this.HandleStateTransition(userId, itemModel, newEvent);
                // Extension resets the notification status.
                if (newEvent == ReservationEventType.ExtensionGranted)
                {
                    item.NotifiedBeforeExpiration = false;
                    item.NotifiedOnExpiration = false;
                    await _unitOfWork.SaveChanges();
                }

                await _unitOfWork.CommitTransaction();
                if (newEvent == ReservationEventType.ExtensionRequested)
                {
                    TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, _) =>
                    {
                        var notificationService = services.GetRequiredService<IBoardGamesNotificationService>();
                        await notificationService.TriggerReservationItemExtensionRequest(itemId);
                    });
                }
                else if (newEvent == ReservationEventType.Assigned)
                {
                    var items = await this.GetReservationItems(reservationId);
                    if (items.All(i => i.State != ReservationItemState.New))
                    {
                        TaskUtils.FireAndForget(_serviceProvider, _logger, async (services, _) =>
                        {
                            var notificationService = services.GetRequiredService<IBoardGamesNotificationService>();
                            await notificationService.TriggerReservationFullyAssigned(reservationId);
                        });
                    }
                }
            }
            catch (InvalidTransitionException)
            {
                _logger.LogError("Invalid item transition, rolling changes back.");
                await _unitOfWork.RollbackTransaction();
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot add new event to a reservation item, rolling changes back.");
                await _unitOfWork.RollbackTransaction();
                throw new ReservationManipulationFailedException();
            }
        }
    }
}
