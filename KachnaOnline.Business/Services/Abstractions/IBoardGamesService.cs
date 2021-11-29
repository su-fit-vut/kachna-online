// IBoardGamesService.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        /// <exception cref="InvalidPlayerRangeException">When the provided player range is invalid.</exception>
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
        /// <exception cref="InvalidPlayerRangeException">When the provided player range is invalid.</exception>
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
        /// Returns a list of user's reservation.
        /// </summary>
        /// <param name="user">ID of user to get reservations of.</param>
        /// <param name="state">Optional state filter.</param>
        /// <returns>A list of <see cref="Reservation"/> made by the <paramref name="user"/> in state
        /// <paramref name="state"/>.</returns>
        Task<ICollection<Reservation>> GetUserReservations(int user, ReservationState? state);

        /// <summary>
        /// Returns a list of all reservations.
        /// </summary>
        /// <param name="state">If not null, filtering based on overall reservation state will be done.</param>
        /// <param name="assignedTo">If not null, filtering based on assigned board game manager will be done.</param>
        /// <returns>A list of <see cref="Reservation"/> in state <paramref name="state"/> assigned to board game
        /// manager <paramref name="assignedTo"/></returns>
        Task<ICollection<Reservation>> GetAllReservations(ReservationState? state, int? assignedTo);

        /// <summary>
        /// Returns a list of items in a reservation.
        /// </summary>
        /// <param name="reservationId">Reservation ID to get items of.</param>
        /// <returns>The list of reservation items within reservation with ID
        /// <paramref name="reservationId"/>.</returns>
        Task<ICollection<ReservationItem>> GetReservationItems(int reservationId);

        /// <summary>
        /// Returns a reservation item with the given ID.
        /// </summary>
        /// <param name="itemId">ID of an item to return.</param>
        /// <returns>An item with ID <paramref name="itemId"/>. Returns null if the item was not found.</returns>
        Task<ReservationItem> GetReservationItem(int itemId);

        /// <summary>
        /// Return an ID of a user who is assigned to the given reservation item.
        /// </summary>
        /// <param name="itemId">A reservation item to get the assignee of.</param>
        /// <returns>The ID of the user who is assigned to the item. Null if the item has not been assigned yet.</returns>
        Task<int?> GetReservationItemAssignee(int itemId);

        /// <summary>
        /// Returns a reservation with the given ID.
        /// </summary>
        /// <param name="reservationId">ID of a reservation to return.</param>
        /// <returns><see cref="Reservation"/> with ID <paramref name="reservationId"/></returns>
        /// <exception cref="ReservationNotFoundException">Thrown when no such reservation exists.</exception>
        Task<Reservation> GetReservation(int reservationId);

        /// <summary>
        /// Creates a new reservation.
        /// </summary>
        /// <param name="reservation"><see cref="Reservation"/> to create.</param>
        /// <param name="createdBy">ID of the user who requested the creation.</param>
        /// <param name="reservationGames">Array of game IDs to reserve.</param>
        /// <returns>The created <see cref="Reservation"/> with a filled ID.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="reservation"/> is null.</exception>
        /// <exception cref="BoardGameNotFoundException">When a requested game does not exist.</exception>
        /// <exception cref="GameUnavailableException">When some of the requested board games are not
        /// available.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be created.</exception>
        /// <exception cref="UserNotFoundException">When a user with ID <paramref name="createdBy"/> does not exist.</exception>
        Task<Reservation> CreateReservation(Reservation reservation, int createdBy, IEnumerable<int> reservationGames);

        /// <summary>
        /// Adds new items to a reservation.
        /// </summary>
        /// <param name="reservationId">ID of the reservation to add items to.</param>
        /// <param name="addedBy">ID of the user who is adding the games.</param>
        /// <param name="newGames">Array of game IDs to reserve.</param>
        /// <exception cref="BoardGameNotFoundException">When a requested game does not exist.</exception>
        /// <exception cref="GameUnavailableException">When some of the requested board games are not
        /// available.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be created.</exception>
        /// <exception cref="ReservationNotFoundException">When a reservation with ID
        /// <paramref name="reservationId"/> does not exist.</exception>
        /// <exception cref="UserNotFoundException">When a user with ID <paramref name="addedBy"/> does not exist.</exception>
        Task AddReservationItems(int reservationId, int addedBy, IEnumerable<int> newGames);

        /// <summary>
        /// Updates user note in a reservation.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="userId">ID of the user requesting the change.</param>
        /// <param name="note">The new user note.</param>
        /// <exception cref="ReservationNotFoundException">When no such reservation exists.</exception>
        /// <exception cref="ReservationAccessDeniedException">When the user does not own the reservation.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be modified.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="note"/> is null.</exception>
        Task UpdateReservationNote(int id, int userId, string note);

        /// <summary>
        /// Updates internal note in a reservation.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="note">The new internal note.</param>
        /// <exception cref="ReservationNotFoundException">When no such reservation exists.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be modified.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="note"/> is null.</exception>
        Task UpdateReservationNoteInternal(int id, string note);

        /// <summary>
        /// Updates discord message ID of a reservation.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="messageId">Discord message ID to save.</param>
        /// <exception cref="ReservationNotFoundException">When no such reservation exists.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be modified.</exception>
        Task UpdateReservationDiscordMessageId(int id, ulong messageId);

        /// <summary>
        /// Returns state history of a single item.
        /// </summary>
        /// <param name="reservationId">ID of reservation the item belongs to.</param>
        /// <param name="itemId">ID of an item to get history of.</param>
        /// <remarks>In the current implementation, the <paramref name="reservationId"/> is somewhat redundant.
        /// Previously, the items were intended as a weak entity, however, currently, all items have a unique ID,
        /// regardless of reservation. Hence in this case, it acts as a sanity check.</remarks>
        /// <returns>The history of <paramref name="itemId"/> from reservation <paramref name="reservationId"/>.
        /// Sorted from the oldest to the newest events.</returns>
        /// <exception cref="ReservationNotFoundException">When no such item exists.</exception>
        Task<ICollection<ReservationItemEvent>> GetItemHistory(int reservationId, int itemId);

        /// <summary>
        /// Returns the latest event of a reservation item.
        /// </summary>
        /// <param name="reservationId">ID of reservation the item belongs to.</param>
        /// <param name="itemId">ID of an item to get history of.</param>
        /// <remarks>See <see cref="GetItemHistory"/> for information about redundancy of <paramref name="itemId"/>
        /// and <see cref="reservationId"/>.</remarks>
        /// <exception cref="ReservationNotFoundException">When no such item exists.</exception>
        /// <returns>The latest event that happened to an item with <paramref name="itemId"/>.</returns>
        Task<ReservationItemEvent> GetLatestEvent(int reservationId, int itemId);

        /// <summary>
        /// Creates a new event which modifies the state of a reservation item.
        /// </summary>
        /// <param name="user">User requesting the change.</param>
        /// <param name="reservationId">ID of reservation the item belongs to.</param>
        /// <param name="itemId">ID of an item to add a new event to.</param>
        /// <remarks>In the current implementation, the <paramref name="reservationId"/> is somewhat redundant.
        /// Previously, the items were intended as a weak entity, however, currently, all items have a unique ID,
        /// regardless of reservation. Hence in this case, it acts as a sanity check.</remarks>
        /// <param name="newEvent">Type of the event to create.</param>
        /// <exception cref="ReservationNotFoundException">When no such item exists.</exception>
        /// <exception cref="InvalidTransitionException">When the requested transition does not make sense in the
        /// current context.</exception>
        /// <exception cref="ReservationAccessDeniedException">When the reservation belongs to another user.</exception>
        /// <exception cref="ReservationManipulationFailedException">When the reservation cannot be modified.</exception>
        Task ModifyItemState(ClaimsPrincipal user, int reservationId, int itemId, ReservationEventType newEvent);
    }
}
