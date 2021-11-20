// BoardGamesReservationsController.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.BoardGames;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("boardGames/reservations")]
    [Authorize]
    public class BoardGamesReservationsController : ControllerBase
    {
        private readonly BoardGamesFacade _facade;

        public BoardGamesReservationsController(BoardGamesFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Returns the list of the authenticated user's reservations.
        /// </summary>
        /// <param name="state">If present, only reservations of this overall state will be returned.</param>
        /// <returns>A list of <see cref="ReservationDto"/>, filtered by state if requested.</returns>
        /// <response code="200">The list of reservations.</response>
        [ProducesResponseType(200)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations(ReservationState? state)
        {
            var userId = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
            return new ActionResult<IEnumerable<ReservationDto>>(await _facade.GetUserReservations(userId, state));
        }

        /// <summary>
        /// Returns the list of all reservations in the system.
        /// </summary>
        /// <param name="state">If present, only reservations of this overall state will be returned.</param>
        /// <returns>A list of all <see cref="ManagerReservationDto"/>, filtered by state if requested.</returns>
        /// <response code="200">The list of all reservations.</response>
        [Authorize(Roles = RoleConstants.BoardGamesManager)]
        [ProducesResponseType(200)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ManagerReservationDto>>> GetAllReservations(ReservationState? state)
        {
            return new ActionResult<IEnumerable<ManagerReservationDto>>(await _facade.GetAllReservations(state, null));
        }

        /// <summary>
        /// Returns the list of all reservations in the system assigned to the specified user.
        /// </summary>
        /// <param name="userId">ID of the user to search in reservation assignments.</param>
        /// <param name="state">If present, only reservations of this overall state will be returned.</param>
        /// <returns>A list of all <see cref="ManagerReservationDto"/> assigned to user with <paramref name="userId"/>,
        /// filtered by state if requested.</returns>
        /// <response code="200">The list of all reservations assigned to the user with ID
        /// <paramref name="userId"/>.</response>
        [Authorize(Roles = RoleConstants.BoardGamesManager)]
        [ProducesResponseType(200)]
        [HttpGet("all/assignedTo/{userId}")]
        public async Task<ActionResult<IEnumerable<ManagerReservationDto>>> GetAssignedReservations(int userId,
            ReservationState? state)
        {
            return new ActionResult<IEnumerable<ManagerReservationDto>>(
                await _facade.GetAllReservations(state, userId));
        }

        /// <summary>
        /// Returns the list of all reservations in the system assigned to the authenticated user.
        /// </summary>
        /// <param name="state">If present, only reservations of this overall state will be returned.</param>
        /// <returns>A list of all <see cref="ManagerReservationDto"/> assigned to the authenticated user,
        /// filtered by state if requested.</returns>
        /// <response code="200">The list of all reservations assigned to the authenticated user.</response>
        [Authorize(Roles = RoleConstants.BoardGamesManager)]
        [ProducesResponseType(200)]
        [HttpGet("all/assignedTo/me")]
        public async Task<ActionResult<IEnumerable<ManagerReservationDto>>> GetAssignedReservations(
            ReservationState? state)
        {
            var userId = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
            return new ActionResult<IEnumerable<ManagerReservationDto>>(
                await _facade.GetAllReservations(state, userId));
        }

        /// <summary>
        /// Returns a reservation with the given ID.
        /// </summary>
        /// <param name="id">ID of the reservation to return.</param>
        /// <returns><see cref="ReservationDto"/> corresponding to ID <paramref name="id"/>. If the user is an
        /// authorized, returns <see cref="ManagerReservationDto"/> instead.</returns>
        /// <response code="200">The reservation.</response>
        /// <response code="403">The user is not a board games manager and it belong to another user.</response>
        /// <response code="404">No such reservation exists.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            try
            {
                return await _facade.GetReservation(this.User, id);
            }
            catch (NotABoardGamesManagerException)
            {
                return this.Forbid();
            }
            catch (ReservationNotFoundException)
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Creates a new reservation.
        /// </summary>
        /// <param name="creationDto"><see cref="CreateReservationDto"/> to create.</param>
        /// <returns>The created <see cref="ReservationDto"/> if the creation succeeded.</returns>
        /// <response code="201">The created reservation.</response>
        /// <response code="404">When a requested game does not exist.</response>
        /// <response code="409">All of the given board games could not be reserved (e.g. are not available).
        /// Returns the first conflicting board game ID.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpPost]
        public async Task<ActionResult<ReservationDto>> CreateReservation(CreateReservationDto creationDto)
        {
            try
            {
                var user = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
                var reservation = await _facade.CreateNewReservation(user, creationDto);
                return this.CreatedAtAction(nameof(this.GetReservation), new { id = reservation.Id }, reservation);
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFound();
            }
            catch (UserNotFoundException)
            {
                // Should not happen, just to be safe.
                return this.NotFound();
            }
            catch (GameUnavailableException e)
            {
                return this.Conflict(e.UnavailableBoardGameId);
            }
            catch (ReservationManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Updates a user note in a reservation with the given ID.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="noteDto"><see cref="ReservationNoteUserDto"/> containing the new user note.</param>
        /// <response code="204">The reservation was updated.</response>
        /// <response code="403">The reservation belongs to another user.</response>
        /// <response code="404">No such reservation exists.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("{id}/note")]
        public async Task<ActionResult> UpdateReservationNote(int id, ReservationNoteUserDto noteDto)
        {
            try
            {
                var userId = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
                await _facade.UpdateReservationNote(id, userId, noteDto);
                return this.NoContent();
            }
            catch (ReservationAccessDeniedException)
            {
                return this.Forbid();
            }
            catch (ReservationNotFoundException)
            {
                return this.NotFound();
            }
            catch (ReservationManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Updates an internal note in a reservation with the given ID.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="noteDto"><see cref="ReservationNoteInternalDto"/> containing the new internal note.</param>
        /// <response code="204">The reservation was updated.</response>
        /// <response code="404">No such reservation exists.</response>
        [Authorize(Roles = RoleConstants.BoardGamesManager)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPut("{id}/noteInternal")]
        public async Task<ActionResult> UpdateReservationNoteInternal(int id, ReservationNoteInternalDto noteDto)
        {
            try
            {
                await _facade.UpdateReservationNoteInternal(id, noteDto);
                return this.NoContent();
            }
            catch (ReservationNotFoundException)
            {
                return this.NotFound();
            }
            catch (ReservationManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Creates a new reservation for a user.
        /// </summary>
        /// <param name="userId">ID of the user to create the reservation for.</param>
        /// <param name="creationDto"><see cref="ManagerCreateReservationDto"/> to create.</param>
        /// <returns>The created <see cref="ManagerReservationDto"/> if the creation succeeded.</returns>
        /// <response code="201">The created reservation.</response>
        /// <response code="404">When a requested game or user to create for does not exist.</response>
        /// <response code="409">All of the given board games could not be reserved (e.g. are not available).
        /// Returns the list of conflicting board game IDs.</response>
        [Authorize(Roles = RoleConstants.BoardGamesManager)]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpPost("madeFor/{userId}")]
        public async Task<ActionResult<ManagerReservationDto>> ManagerCreateReservation(int userId,
            ManagerCreateReservationDto creationDto)
        {
            try
            {
                var creatingUser = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
                var reservation = await _facade.ManagerCreateNewReservation(creatingUser, userId, creationDto);
                return this.CreatedAtAction(nameof(this.GetReservation), new { id = reservation.Id }, reservation);
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFound();
            }
            catch (UserNotFoundException)
            {
                return this.NotFound();
            }
            catch (GameUnavailableException e)
            {
                return this.Conflict(e.UnavailableBoardGameId);
            }
            catch (ReservationManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Adds extra board games to a reservation with the given ID.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="newItems"><see cref="UpdateReservationItemsDto"/> containing the items to be added.</param>
        /// <response code="204">The reservation was updated.</response>
        /// <response code="404">No such reservation exists or a requested game does not exist.</response>
        /// <response code="409">All of the given board games could not be reserved (e.g. are not available).
        /// Returns the list of conflicting board game IDs.</response>
        [Authorize(Roles = RoleConstants.BoardGamesManager)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpPut("{id}/items")]
        public async Task<ActionResult> UpdateReservationItems(int id, UpdateReservationItemsDto newItems)
        {
            try
            {
                var user = int.Parse(this.User.FindFirstValue(IdentityConstants.IdClaim));
                await _facade.AddReservationItems(id, user, newItems);
                return this.NoContent();
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFound();
            }
            catch (ReservationNotFoundException)
            {
                return this.NotFound();
            }
            catch (UserNotFoundException)
            {
                // Should not happen, just to be safe.
                return this.NotFound();
            }
            catch (GameUnavailableException e)
            {
                return this.Conflict(e.UnavailableBoardGameId);
            }
            catch (ReservationManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Retrieves history of a reservation item.
        /// </summary>
        /// <param name="id">ID of the reservation.</param>
        /// <param name="itemId">ID of the item in the reservation.</param>
        /// <returns>A list of <see cref="ReservationItemEventDto"/> representing the whole history, sorted
        /// chronologically.</returns>
        /// <response code="200">The item history.</response>
        /// <response code="404">No such item or reservation exists.</response>
        [Authorize(Roles = RoleConstants.BoardGamesManager)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}/{itemId}/events")]
        public async Task<ActionResult<IEnumerable<ReservationItemEventDto>>> GetItemHistory(int id, int itemId)
        {
            try
            {
                return new ActionResult<IEnumerable<ReservationItemEventDto>>(await _facade.GetItemHistory(id, itemId));
            }
            catch (ReservationNotFoundException)
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Modifies the state of a reservation item.
        /// </summary>
        /// <param name="id">ID of the reservation.</param>
        /// <param name="itemId">ID of the item in the reservation.</param>
        /// <param name="type">Type of the modification.</param>
        /// <response code="204">Item state updated.</response>
        /// <response code="404">No such item or reservation exists.</response>
        /// <response code="409">Such modification is not possible.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpPost("{id}/{itemId}/events")]
        public async Task<ActionResult> ModifyItemState(int id, int itemId, ReservationEventType type)
        {
            throw new NotImplementedException();
        }
    }
}
