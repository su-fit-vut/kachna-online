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
using KachnaOnline.Business.Extensions;
using KachnaOnline.Dto.Users;
using Microsoft.AspNetCore.Http;
using ReservationState = KachnaOnline.Dto.BoardGames.ReservationState;
using ReservationEventType = KachnaOnline.Dto.BoardGames.ReservationEventType;
using ReservationEventModelType = KachnaOnline.Business.Models.BoardGames.ReservationEventType;

namespace KachnaOnline.Business.Facades
{
    public class BoardGamesFacade
    {
        private readonly IMapper _mapper;
        private readonly IBoardGamesService _boardGamesService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoardGamesFacade(IBoardGamesService boardGamesService, IMapper mapper, IUserService userService,
            IHttpContextAccessor httpContextAccessor)
        {
            _boardGamesService = boardGamesService;
            _mapper = mapper;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        private bool IsUserManager
            => _httpContextAccessor.HttpContext?.User?.IsInRole(AuthConstants.BoardGamesManager) ?? false;

        private async Task<MadeByUserDto> MakeMadeByDto(int? userId)
        {
            return await _userService.GetUserMadeByDto(userId, this.IsUserManager);
        }

        /// <summary>
        /// Returns an array of reservation items with filled in assignees if available.
        /// </summary>
        /// <param name="reservationId">ID of the reservation to get the items of.</param>
        /// <returns>An array of items in a reservation with ID <paramref name="reservationId"/>.</returns>
        private async Task<ReservationItemDto[]> GetReservationItems(int reservationId)
        {
            var items = new List<ReservationItemDto>();
            foreach (var item in await _boardGamesService.GetReservationItems(reservationId))
            {
                var itemDto = _mapper.Map<ReservationItemDto>(item);
                itemDto.AssignedTo =
                    await this.MakeMadeByDto(await _boardGamesService.GetReservationItemAssignee(itemDto.Id));
                items.Add(itemDto);
            }

            return items.ToArray();
        }

        /// <summary>
        /// Returns all categories of board games.
        /// </summary>
        /// <returns>A list of <see cref="CategoryDto"/>.</returns>
        public async Task<List<CategoryDto>> GetCategories()
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
            if (!user.IsInRole(AuthConstants.BoardGamesManager))
                visible = true;

            var games = new List<BoardGameDto>();
            foreach (var game in await _boardGamesService.GetBoardGames(categoryId, players, available, visible))
            {
                if (user.IsInRole(AuthConstants.BoardGamesManager))
                {
                    var managerDto = _mapper.Map<ManagerBoardGameDto>(game);
                    managerDto.Owner = await this.MakeMadeByDto(game.OwnerId);
                    games.Add(managerDto);
                }
                else
                {
                    games.Add(_mapper.Map<BoardGameDto>(game));
                }
            }

            return games;
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
                    throw new NotAuthenticatedException();

                if (!user.IsInRole(AuthConstants.BoardGamesManager))
                    throw new NotABoardGamesManagerException();
            }

            if (user.IsInRole(AuthConstants.BoardGamesManager))
            {
                var managerDto = _mapper.Map<ManagerBoardGameDto>(game);
                managerDto.Owner = await this.MakeMadeByDto(game.OwnerId);
                return managerDto;
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
            var createdDto = _mapper.Map<ManagerBoardGameDto>(createdGame);
            createdDto.Owner = await this.MakeMadeByDto(createdGame.OwnerId);
            return createdDto;
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

        /// <summary>
        /// Returns a list of all user's reservation.
        /// </summary>
        /// <param name="user">User to get reservations of.</param>
        /// <param name="state">Optional reservation filter based on state.</param>
        /// <returns>The list of user's reservation.</returns>
        public async Task<List<ReservationDto>> GetUserReservations(int user,
            ReservationState? state)
        {
            var stateModel = _mapper.Map<Models.BoardGames.ReservationState?>(state);
            var dtos = new List<ReservationDto>();
            foreach (var reservation in await _boardGamesService.GetUserReservations(user, stateModel))
            {
                var dto = _mapper.Map<ReservationDto>(reservation);
                dto.Items = await this.GetReservationItems(dto.Id);
                dtos.Add(dto);
            }

            return dtos;
        }

        /// <summary>
        /// Returns a list of all reservations.
        /// </summary>
        /// <param name="state">Optional reservation filter based on state.</param>
        /// <param name="assignedTo">Optional reservation filter based on assigned board games manager ID.</param>
        /// <returns>The list of all board games reservations, filtered by the given filters if requested.</returns>
        public async Task<List<ManagerReservationDto>> GetAllReservations(ReservationState? state,
            int? assignedTo)
        {
            var stateModel = _mapper.Map<Models.BoardGames.ReservationState?>(state);
            var dtos = new List<ManagerReservationDto>();
            foreach (var reservation in await _boardGamesService.GetAllReservations(stateModel, assignedTo))
            {
                var dto = _mapper.Map<ManagerReservationDto>(reservation);
                dto.Items = await this.GetReservationItems(dto.Id);
                dtos.Add(dto);
            }

            return dtos;
        }

        /// <summary>
        /// Returns a single reservation.
        /// </summary>
        /// <param name="user">Currently authenticated user.</param>
        /// <param name="reservationId">ID of the reservation to return.</param>
        /// <returns>Reservation with ID <paramref name="reservationId"/>. Returns a
        /// <see cref="ManagerReservationDto"/> if the requesting <paramref name="user"/>"/> is a board games
        /// manager.</returns>
        /// <exception cref="NotABoardGamesManagerException">Thrown when <paramref name="user"/> is not a board games
        /// manager and the reservation is owned by someone else.</exception>
        /// <exception cref="ReservationNotFoundException">When a reservation with <paramref name="reservationId"/>
        /// does not exist.</exception>
        public async Task<ReservationDto> GetReservation(ClaimsPrincipal user, int reservationId)
        {
            var reservation = await _boardGamesService.GetReservation(reservationId);
            if (user.IsInRole(AuthConstants.BoardGamesManager))
            {
                var managerDto = _mapper.Map<ManagerReservationDto>(reservation);
                managerDto.Items = await this.GetReservationItems(managerDto.Id);
                return managerDto;
            }

            if (reservation.MadeById != int.Parse(user.FindFirstValue(IdentityConstants.IdClaim)))
                throw new NotABoardGamesManagerException();

            var userDto = _mapper.Map<ReservationDto>(reservation);
            userDto.Items = await this.GetReservationItems(userDto.Id);
            return userDto;
        }

        /// <summary>
        /// Updates user note in a reservation.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="userId">ID of the user requesting the change.</param>
        /// <param name="note"><see cref="ReservationNoteUserDto"/> containing the new user note.</param>
        /// <exception cref="ReservationNotFoundException">When no such reservation exists.</exception>
        /// <exception cref="ReservationAccessDeniedException">When the user does not own the reservation.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be modified.</exception>
        public async Task UpdateReservationNote(int id, int userId, ReservationNoteUserDto note)
        {
            await _boardGamesService.UpdateReservationNote(id, userId, note.NoteUser);
        }

        /// <summary>
        /// Updates internal note in a reservation.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="note"><see cref="ReservationNoteInternalDto"/> containing the new internal note.</param>
        /// <exception cref="ReservationNotFoundException">When no such reservation exists.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be modified.</exception>
        public async Task UpdateReservationNoteInternal(int id, ReservationNoteInternalDto note)
        {
            await _boardGamesService.UpdateReservationNoteInternal(id, note.NoteInternal);
        }

        /// <summary>
        /// Returns state history of a single item.
        /// </summary>
        /// <param name="reservationId">ID of reservation the item belongs to.</param>
        /// <param name="itemId">ID of an item to get history of.</param>
        /// <returns>List of <see cref="ReservationItemEventDto"/> sorted from oldest to newest.</returns>
        /// <exception cref="ReservationNotFoundException">When no such item exists.</exception>
        public async Task<List<ReservationItemEventDto>> GetItemHistory(int reservationId, int itemId)
        {
            var events = await _boardGamesService.GetItemHistory(reservationId, itemId);
            var eventsDto = new List<ReservationItemEventDto>();
            foreach (var eventModel in events)
            {
                var eventDto = _mapper.Map<ReservationItemEventDto>(eventModel);
                eventDto.MadeBy = await this.MakeMadeByDto(eventModel.MadeById);
                eventsDto.Add(eventDto);
            }

            return eventsDto;
        }

        /// <summary>
        /// Creates a new reservation for a user.
        /// </summary>
        /// <param name="userId">ID of the user requesting the reservation.</param>
        /// <param name="reservation"><see cref="CreateReservationDto"/> containing the requested games.</param>
        /// <returns>The created <see cref="ReservationDto"/>.</returns>
        /// <exception cref="GameUnavailableException">When the whole request cannot be satisfied due to a game
        /// not being available.</exception>
        /// <exception cref="BoardGameNotFoundException">When a requested game does not exist.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation created failed.</exception>
        /// <exception cref="UserNotFoundException">When a user with ID <paramref name="userId"/> does not exist.
        /// Should not normally happen.</exception>
        public async Task<ReservationDto> CreateNewReservation(int userId, CreateReservationDto reservation)
        {
            var reservationModel = _mapper.Map<Reservation>(reservation);
            reservationModel.MadeById = userId;
            var created =
                await _boardGamesService.CreateReservation(reservationModel, userId, reservation.BoardGameIds);
            var createdDto = _mapper.Map<ReservationDto>(created);
            createdDto.Items = await this.GetReservationItems(createdDto.Id);
            return createdDto;
        }

        /// <summary>
        /// Creates a new reservation for a user by someone else (a board games manager).
        /// </summary>
        /// <param name="madeBy">ID of the user who is creating the reservation.</param>
        /// <param name="madeFor">ID of the user who the games are reserved for.</param>
        /// <param name="reservation"><see cref="ManagerCreateReservationDto"/> containing the requested games.</param>
        /// <returns>The created <see cref="ManagerReservationDto"/>.</returns>
        /// <exception cref="GameUnavailableException">When the whole request cannot be satisfied due to a game
        /// not being available.</exception>
        /// <exception cref="BoardGameNotFoundException">When a request game does not exist.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation created failed.</exception>
        /// <exception cref="UserNotFoundException">When a user with ID <paramref name="madeFor"/> does not exist.</exception>
        public async Task<ManagerReservationDto> ManagerCreateNewReservation(int madeBy, int madeFor,
            ManagerCreateReservationDto reservation)
        {
            var reservationModel = _mapper.Map<Reservation>(reservation);
            reservationModel.MadeById = madeFor;
            var created =
                await _boardGamesService.CreateReservation(reservationModel, madeBy, reservation.BoardGameIds);
            var createdDto = _mapper.Map<ManagerReservationDto>(created);
            createdDto.Items = await this.GetReservationItems(createdDto.Id);
            return createdDto;
        }

        /// <summary>
        /// Adds new items to a reservation.
        /// </summary>
        /// <param name="reservationId">ID of the reservation to add items to.</param>
        /// <param name="addedBy">ID of the user who is adding the games.</param>
        /// <param name="items"><see cref="UpdateReservationItemsDto"/> containing the new items.</param>
        /// <exception cref="BoardGameNotFoundException">When a requested game does not exist.</exception>
        /// <exception cref="GameUnavailableException">When some of the requested board games are not
        /// available.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be created.</exception>
        /// <exception cref="ReservationNotFoundException">When a reservation with ID
        /// <paramref name="reservationId"/> does not exist.</exception>
        /// <exception cref="UserNotFoundException">When a user with ID <paramref name="addedBy"/> does not exist.</exception>
        public async Task AddReservationItems(int reservationId, int addedBy, UpdateReservationItemsDto items)
        {
            await _boardGamesService.AddReservationItems(reservationId, addedBy, items.BoardGameIds);
        }

        /// <summary>
        /// Creates a new event which modifies the state of a reservation item.
        /// </summary>
        /// <param name="user">User requesting the state change.</param>
        /// <param name="reservationId">ID of reservation the item belongs to.</param>
        /// <param name="itemId">ID of an item to add a new event to.</param>
        /// <param name="eventType">Type of the event to create.</param>
        /// <exception cref="ReservationNotFoundException">When no such item exists.</exception>
        /// <exception cref="InvalidTransitionException">When the requested transition does not make sense in the
        /// current context.</exception>
        /// <exception cref="NotABoardGamesManagerException">When the <paramref name="eventType"/> event can only be
        /// performed by a board games manager.</exception>
        /// <exception cref="ReservationAccessDeniedException">When the reservation belongs to another user.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be modified.</exception>
        public async Task ModifyItemState(ClaimsPrincipal user, int reservationId, int itemId,
            ReservationEventType eventType)
        {
            // Created is implicit and can only be one.
            if (eventType == ReservationEventType.Created)
                throw new InvalidTransitionException();

            if (eventType is ReservationEventType.Assigned or ReservationEventType.HandedOver or
                ReservationEventType.ExtensionGranted or ReservationEventType.ExtensionRefused or
                ReservationEventType.Returned && !user.IsInRole(AuthConstants.BoardGamesManager))
                throw new NotABoardGamesManagerException();

            var stateModel = _mapper.Map<ReservationEventModelType>(eventType);
            await _boardGamesService.ModifyItemState(user, reservationId, itemId, stateModel);
        }
    }
}
