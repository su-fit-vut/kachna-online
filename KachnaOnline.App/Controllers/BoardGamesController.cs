// BoardGamesController.cs
// Author: František Nečas

using System.Collections.Generic;
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
    [Route("boardGames")]
    [Authorize(Roles = RoleConstants.BoardGamesManager)]
    public class BoardGamesController : ControllerBase
    {
        private readonly BoardGamesFacade _facade;

        public BoardGamesController(BoardGamesFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Returns the current list of board games.
        /// </summary>
        /// <remarks>
        /// The queries can be combined in any way.
        /// </remarks>
        /// <param name="categoryId">If present, only games from this category will be returned.</param>
        /// <param name="players">If present, only games which can be played by this number of players will be
        /// returned.</param>
        /// <param name="available">If present, only games with this availability will be returned.</param>
        /// <param name="visible">If present, only games with this visibility will be returned.
        /// Regular and unauthenticated users will always receive only visible games.</param>
        /// <returns>A list of <see cref="BoardGameDto"/> corresponding to the given queries. If the user
        /// is an authorized board games manager, returns a list of <see cref="ManagerBoardGameDto"/></returns>
        /// <response code="200">The list of board games.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardGameDto>>> GetBoardGames(
            int? categoryId,
            int? players,
            bool? available,
            bool? visible)
        {
            var dto = await _facade.GetBoardGames(this.User, categoryId, players, available, visible);
            return new ActionResult<IEnumerable<BoardGameDto>>(dto);
        }

        /// <summary>
        /// Returns a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to return.</param>
        /// <returns>A <see cref="BoardGameDto"/> of a game corresponding to ID <paramref name="id"/>.
        /// A <see cref="ManagerBoardGameDto"/> if the user is an authorized board games manager.</returns>
        /// <response code="200">The board game.</response>
        /// <response code="401">The board game exists but is not visible and the user is not authenticated.</response>
        /// <response code="403">The board game exists but is not visible and the user is not
        /// a board games manager.</response>
        /// <response code="404">No such board game exists.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardGameDto>> GetBoardGame(int id)
        {
            try
            {
                return await _facade.GetBoardGame(this.User, id);
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFound();
            }
            catch (NotAuthenticatedException)
            {
                return this.Unauthorized();
            }
            catch (NotABoardGamesManagerException)
            {
                return this.Forbid();
            }
        }

        /// <summary>
        /// Creates a new board game.
        /// </summary>
        /// <param name="game"><see cref="CreateBoardGameDto"/> to create.</param>
        /// <returns>The created <see cref="ManagerBoardGameDto"/> if the creation succeeded.</returns>
        /// <response code="201">The created game.</response>
        /// <response code="422">A category or user with the given ID does not exist.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(422)]
        [HttpPost]
        public async Task<ActionResult<ManagerBoardGameDto>> CreateBoardGame(CreateBoardGameDto game)
        {
            try
            {
                var createdGame = await _facade.CreateBoardGame(game);
                return this.CreatedAtAction(nameof(this.GetBoardGame), new { id = createdGame.Id }, createdGame);
            }
            catch (NotABoardGamesManagerException)
            {
                return this.Forbid();
            }
            catch (CategoryNotFoundException)
            {
                return this.UnprocessableEntity();
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntity();
            }
            catch (BoardGameManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Updates a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="game"><see cref="CreateBoardGameDto"/> representing the new state.</param>
        /// <response code="204">Board game was updated.</response>
        /// <response code="404">Board game with the given ID does not exist.</response>
        /// <response code="422">A category or user with the given ID does not exist.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBoardGame(int id, CreateBoardGameDto game)
        {
            try
            {
                await _facade.UpdateBoardGame(id, game);
                return this.NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                return this.Forbid();
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFound();
            }
            catch (CategoryNotFoundException)
            {
                return this.UnprocessableEntity();
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntity();
            }
            catch (BoardGameManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Updates stock of a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="stock"><see cref="BoardGameStockDto"/> representing the new stock state.</param>
        /// <response code="204">Board game stock was updated.</response>
        /// <response code="404">Board game with the given ID does not exist.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPut("{id}/stock")]
        public async Task<ActionResult> UpdateBoardGameStock(int id, BoardGameStockDto stock)
        {
            try
            {
                await _facade.UpdateBoardGameStock(id, stock);
                return this.NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                return this.Forbid();
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFound();
            }
            catch (BoardGameManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Returns the list of all board game categories.
        /// </summary>
        /// <returns>A list of <see cref="CategoryDto"/>.</returns>
        /// <response code="200">The list of board game categories.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            return new ActionResult<IEnumerable<CategoryDto>>(await _facade.GetCategories());
        }

        /// <summary>
        /// Returns a board game category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to return.</param>
        /// <returns>A <see cref="CategoryDto"/> of a category corresponding to ID <paramref name="id"/>.</returns>
        /// <response code="200">The category.</response>
        /// <response code="404">No such category exists.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("categories/{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            try
            {
                return await _facade.GetCategory(id);
            }
            catch (CategoryNotFoundException)
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Creates a new board game category.
        /// </summary>
        /// <param name="category"><see cref="CreateCategoryDto"/> to create.</param>
        /// <returns>The created <see cref="CategoryDto"/> if the creation succeeded.</returns>
        /// <response code="201">The created category.</response>
        [ProducesResponseType(201)]
        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto category)
        {
            try
            {
                var createdCategory = await _facade.CreateCategory(category);
                return this.CreatedAtAction(nameof(this.GetCategory), new { id = createdCategory.Id }, createdCategory);
            }
            catch (NotABoardGamesManagerException)
            {
                return this.Forbid();
            }
            catch (CategoryManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Updates a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to update.</param>
        /// <param name="category"><see cref="CreateCategoryDto"/> representing the new state.</param>
        /// <response code="204">Category was updated.</response>
        /// <response code="404">Category with the given ID does not exist.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPut("categories/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, CreateCategoryDto category)
        {
            try
            {
                await _facade.UpdateCategory(id, category);
                return this.NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                return this.Forbid();
            }
            catch (CategoryNotFoundException)
            {
                return this.NotFound();
            }
            catch (CategoryManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
        }

        /// <summary>
        /// Deletes a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to delete.</param>
        /// <response code="204">Category was deleted.</response>
        /// <response code="404">Category with the given ID does not exist.</response>
        /// <response code="409">Board games from this category must first be transferred. Returns the list
        /// of IDs of conflicting board games.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                await _facade.DeleteCategory(id);
                return this.NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                return this.Forbid();
            }
            catch (CategoryNotFoundException)
            {
                return this.NotFound();
            }
            catch (CategoryManipulationFailedException)
            {
                return this.Problem(statusCode: 500);
            }
            catch (CategoryHasBoardGamesException e)
            {
                return this.Conflict(e.ConflictingGameIds);
            }
        }
    }
}
