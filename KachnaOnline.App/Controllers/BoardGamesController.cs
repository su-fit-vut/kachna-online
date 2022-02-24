using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Exceptions;
using KachnaOnline.Business.Exceptions.BoardGames;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.BoardGames;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [ApiController]
    [Route("boardGames")]
    [Authorize(Roles = AuthConstants.BoardGamesManager)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        /// <remarks>
        /// A `ManagerBoardGameDto` is returned if the user is an authorized board games manager.
        /// </remarks>
        /// <param name="id">The ID of the board game to return.</param>
        /// <response code="200">The board game.</response>
        /// <response code="401">The board game exists but is not visible and the user is not authenticated.</response>
        /// <response code="403">The board game exists but is not visible and the user is not
        /// a board games manager.</response>
        /// <response code="404">No such board game exists.</response>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardGameDto>> GetBoardGame(int id)
        {
            try
            {
                return await _facade.GetBoardGame(this.User, id);
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFoundProblem("The specified board game does not exist.");
            }
            catch (NotAuthenticatedException)
            {
                return this.UnauthorizedProblem(
                    "The board game exists but it is not visible to unauthenticated users.",
                    "Board game not visible");
            }
            catch (NotABoardGamesManagerException)
            {
                return this.ForbiddenProblem(
                    "The board game exists but it is not visible to the currently authenticated user.",
                    "Board game not visible");
            }
        }

        /// <summary>
        /// Creates a new board game.
        /// </summary>
        /// <param name="game">A model of the game to create.</param>
        /// <response code="201">The created game.</response>
        /// <response code="422">A category or user with the given ID does not exist or the number of minimum
        /// players is larger than the number of maximum players.</response>
        [ProducesResponseType(typeof(ManagerBoardGameDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPost]
        public async Task<ActionResult<ManagerBoardGameDto>> CreateBoardGame(CreateBoardGameDto game)
        {
            if (game.PlayersMin.HasValue && game.PlayersMax.HasValue && game.PlayersMin.Value > game.PlayersMax.Value)
                return this.UnprocessableEntityProblem("Invalid player range provided.");
            try
            {
                var createdGame = await _facade.CreateBoardGame(game);
                return this.CreatedAtAction(nameof(this.GetBoardGame), new { id = createdGame.Id }, createdGame);
            }
            catch (CategoryNotFoundException)
            {
                return this.UnprocessableEntityProblem("The specified category does not exist.");
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntityProblem("The specified user does not exist.");
            }
            catch (InvalidPlayerRangeException)
            {
                return this.UnprocessableEntityProblem("Invalid player range provided.");
            }
        }

        /// <summary>
        /// Updates a board game with the given ID.
        /// </summary>
        /// <param name="id">The ID of the board game to update.</param>
        /// <param name="game">A model representing the new state.</param>
        /// <response code="204">Board game was updated.</response>
        /// <response code="404">Board game with the given ID does not exist.</response>
        /// <response code="422">A category or user with the given ID does not exist or the number of minimum
        /// players is larger than the number of maximum players.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoardGame(int id, CreateBoardGameDto game)
        {
            if (game.PlayersMin.HasValue && game.PlayersMax.HasValue && game.PlayersMin.Value > game.PlayersMax.Value)
                return this.UnprocessableEntityProblem("Invalid player range provided.");
            try
            {
                await _facade.UpdateBoardGame(id, game);
                return this.NoContent();
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFoundProblem("The specified board game does not exist.");
            }
            catch (CategoryNotFoundException)
            {
                return this.UnprocessableEntityProblem("The specified category does not exist.");
            }
            catch (UserNotFoundException)
            {
                return this.UnprocessableEntityProblem("The specified user does not exist.");
            }
            catch (InvalidPlayerRangeException)
            {
                return this.UnprocessableEntityProblem("Invalid player range provided.");
            }
        }

        /// <summary>
        /// Updates the stock status of a board game with the given ID.
        /// </summary>
        /// <param name="id">The ID of the board game to update.</param>
        /// <param name="stock">A model representing the new stock state.</param>
        /// <response code="204">Board game stock was updated.</response>
        /// <response code="404">Board game with the given ID does not exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateBoardGameStock(int id, BoardGameStockDto stock)
        {
            try
            {
                await _facade.UpdateBoardGameStock(id, stock);
                return this.NoContent();
            }
            catch (BoardGameNotFoundException)
            {
                return this.NotFoundProblem("The specified board game does not exist.");
            }
        }

        /// <summary>
        /// Returns a list of all board game categories.
        /// </summary>
        /// <response code="200">The list of board game categories.</response>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            return await _facade.GetCategories();
        }

        /// <summary>
        /// Returns a board game category with the given ID.
        /// </summary>
        /// <param name="id">The ID of the category to return.</param>
        /// <response code="200">The category.</response>
        /// <response code="404">No such category exists.</response>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("categories/{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            try
            {
                return await _facade.GetCategory(id);
            }
            catch (CategoryNotFoundException)
            {
                return this.NotFoundProblem("The specified category does not exist.");
            }
        }

        /// <summary>
        /// Creates a new board game category.
        /// </summary>
        /// <param name="category">A model of the category to create.</param>
        /// <response code="201">The created category.</response>
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto category)
        {
            var createdCategory = await _facade.CreateCategory(category);
            return this.CreatedAtAction(nameof(this.GetCategory), new { id = createdCategory.Id }, createdCategory);
        }

        /// <summary>
        /// Updates a category with the given ID.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="category">A model representing the new state.</param>
        /// <response code="204">Category was updated.</response>
        /// <response code="404">Category with the given ID does not exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto category)
        {
            try
            {
                await _facade.UpdateCategory(id, category);
                return this.NoContent();
            }
            catch (CategoryNotFoundException)
            {
                return this.NotFoundProblem("The specified board game does not exist.");
            }
        }

        /// <summary>
        /// Deletes a category with the given ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <response code="204">Category was deleted.</response>
        /// <response code="404">Category with the given ID does not exist.</response>
        /// <response code="409">Board games from this category must first be transferred. Returns the list
        /// of IDs of conflicting board games.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(int[]), StatusCodes.Status409Conflict)]
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _facade.DeleteCategory(id);
                return this.NoContent();
            }
            catch (NotABoardGamesManagerException)
            {
                // Shouldn't happen
                return this.ForbiddenProblem();
            }
            catch (CategoryNotFoundException)
            {
                return this.NotFoundProblem("The specified category does not exist.");
            }
            catch (CategoryHasBoardGamesException e)
            {
                return this.Conflict(e.ConflictingGameIds);
            }
        }
    }
}
