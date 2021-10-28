// BoardGameController.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.BoardGames;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("boardGames")]
    public class BoardGameController : ControllerBase
    {
        private readonly BoardGameFacade _facade;

        public BoardGameController(BoardGameFacade facade)
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
        /// <returns>List of <see cref="BoardGameDto"/> corresponding to the given queries.</returns>
        /// <response code="200">Returns the list of board games.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<List<BoardGameDto>>> GetBoardGames(
            [FromQuery] int? categoryId,
            [FromQuery] int? players,
            [FromQuery] bool? available,
            [FromQuery] bool? visible)
        {
            return await _facade.GetBoardGames(this.User, categoryId, players, available, visible);
        }

        /// <summary>
        /// Returns a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to return.</param>
        /// <returns><see cref="BoardGameDto"/> of a game corresponding to ID <paramref name="id"/>.</returns>
        /// <response code="200">Returns the board game.</response>
        /// <response code="404">No such board game exists.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardGameDto>> GetBoardGame(int id)
        {
            try
            {
                return await _facade.GetBoardGame(this.User, id);
            }
            catch (BoardGameNotFoundException)
            {
                return NotFound();
            }
        }
        
        /// <summary>
        /// Creates a new board game.
        /// </summary>
        /// <param name="game"><see cref="BoardGameDto"/> to create.</param>
        /// <returns>The created <see cref="BoardGameDto"/> if the creation succeeded.</returns>
        /// <response code="201">Success, returns the created game.</response>
        /// <response code="400">The given JSON object is not in a valid format.</response>
        /// <response code="403">User not logged in or not a board game manager.</response>
        /// <response code="422">A category or user with the given ID does not exist.</response>
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [Produces("application/json")]
        [HttpPost]
        public async Task<ActionResult<BoardGameDto>> CreateBoardGame([FromBody] BoardGameDto game)
        {
            try
            {
                var createdGame = await _facade.CreateBoardGame(this.User, game);
                return CreatedAtAction(nameof(GetBoardGame), new { id = createdGame.Id }, createdGame);
            }
            catch (NotABoardGamesManagerException)
            {
                return Forbid();
            }
            catch (CategoryNotFoundException)
            {
                return UnprocessableEntity();
            }
            catch (UserNotFoundException)
            {
                return UnprocessableEntity();
            }
            catch (BoardGameManipulationFailedException e)
            {
                return Problem(statusCode: 500, detail: e.Message);
            }
        }
        
        /// <summary>
        /// Updates a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="game"><see cref="BoardGameDto"/> representing the new state.</param>
        /// <response code="204">Success, board game was updated.</response>
        /// <response code="400">The given JSON object is not in a valid format.</response>
        /// <response code="403">User not logged in or not a board game manager.</response>
        /// <response code="404">Board game with the given ID does not exist.</response>
        /// <response code="422">A category or user with the given ID does not exist.</response>
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutBoardGame(int id, [FromBody] BoardGameDto game)
        {
            try
            {
                await _facade.UpdateBoardGame(this.User, id, game);
                return NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                return Forbid();
            }
            catch (BoardGameNotFoundException)
            {
                return NotFound();
            }
            catch (CategoryNotFoundException)
            {
                return UnprocessableEntity();
            }
            catch (UserNotFoundException)
            {
                return UnprocessableEntity();
            }
            catch (BoardGameManipulationFailedException e)
            {
                return Problem(statusCode: 500, detail: e.Message);
            }
        }
        
        /// <summary>
        /// Updates stock of a board game with the given ID.
        /// </summary>
        /// <param name="id">ID of the board game to update.</param>
        /// <param name="stock"><see cref="BoardGameStockDto"/> representing the new stock state.</param>
        /// <response code="204">Success, board game stock was updated.</response>
        /// <response code="400">The given JSON object is not in a valid format.</response>
        /// <response code="403">User not logged in or not a board game manager.</response>
        /// <response code="404">Board game with the given ID does not exist.</response>
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("{id}/stock")]
        public async Task<ActionResult> PutBoardGameStock(int id, [FromBody] BoardGameStockDto stock)
        {
            try
            {
                await _facade.UpdateBoardGameStock(this.User, id, stock);
                return NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                return Forbid();
            }
            catch (BoardGameNotFoundException)
            {
                return NotFound();
            }
            catch (BoardGameManipulationFailedException e)
            {
                return Problem(statusCode: 500, detail: e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e);
            }
        }
        
        /// <summary>
        /// Returns the list of all board game categories.
        /// </summary>
        /// <returns>List of <see cref="CategoryDto"/>.</returns>
        /// <response code="200">Returns the list of board game categories.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [Produces("application/json")]
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            return await _facade.GetCategories();
        }

        /// <summary>
        /// Returns a board game category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to return.</param>
        /// <returns><see cref="CategoryDto"/> of a category corresponding to ID <paramref name="id"/>.</returns>
        /// <response code="200">Returns the category.</response>
        /// <response code="404">No such category exists.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        [HttpGet("categories/{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            try
            {
                return await _facade.GetCategory(id);
            }
            catch (CategoryNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Creates a new board game category.
        /// </summary>
        /// <param name="category"><see cref="CategoryDto"/> to create.</param>
        /// <returns>The created <see cref="CategoryDto"/> if the creation succeeded.</returns>
        /// <response code="201">Success, returns the created category.</response>
        /// <response code="400">The given JSON object is not in a valid format.</response>
        /// <response code="403">User not logged in or not a board game manager.</response>
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        [Produces("application/json")]
        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryDto category)
        {
            try
            {
                var createdCategory = await _facade.CreateCategory(this.User, category);
                return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
            }
            catch (NotABoardGamesManagerException)
            {
                return Forbid();
            }
            catch (CategoryManipulationFailedException e)
            {
                return Problem(statusCode: 500, detail: e.Message);
            }
        }

        /// <summary>
        /// Updates a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to update.</param>
        /// <param name="category"><see cref="CategoryDto"/> representing the new state.</param>
        /// <response code="204">Success, category was updated.</response>
        /// <response code="400">The given JSON object is not in a valid format.</response>
        /// <response code="403">User not logged in or not a board game manager.</response>
        /// <response code="404">Category with the given ID does not exist.</response>
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("categories/{id}")]
        public async Task<ActionResult> PutCategory(int id, [FromBody] CategoryDto category)
        {
            try
            {
                await _facade.UpdateCategory(this.User, id, category);
                return NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                return Forbid();
            }
            catch (CategoryNotFoundException)
            {
                return NotFound();
            }
            catch (CategoryManipulationFailedException e)
            {
                return Problem(statusCode: 500, detail: e.Message);
            }
        }

        /// <summary>
        /// Deletes a category with the given ID.
        /// </summary>
        /// <param name="id">ID of the category to delete.</param>
        /// <response code="204">Success, category was deleted.</response>
        /// <response code="403">User not logged in or not a board game manager.</response>
        /// <response code="404">Category with the given ID does not exist.</response>
        /// <response code="409">Board games from this category must first be transferred. Returns the list
        /// of conflicting board games.</response>
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [Produces("application/json")]
        [HttpDelete("categories/{id}")]
        public async Task<ActionResult<List<BoardGameDto>>> DeleteCategory(int id)
        {
            try
            {
                await _facade.DeleteCategory(this.User, id);
                return NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                return Forbid();
            }
            catch (CategoryNotFoundException)
            {
                return NotFound();
            }
            catch (CategoryManipulationFailedException e)
            {
                return Problem(statusCode: 500, detail: e.Message);
            }
            catch (CategoryHasBoardGamesException e)
            {
                return Conflict(e.ConflictingGamesDto);
            }
        }
    }
}
