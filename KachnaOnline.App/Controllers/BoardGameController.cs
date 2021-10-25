// BoardGameController.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// Sample requests:
        /// 
        ///     GET /boardGames
        ///     GET /boardGames?categoryId=1
        /// </remarks>
        /// <param name="categoryId">Optional ID of a <see cref="CategoryDto"/> to limit the list of returned
        /// games to only this category.</param>
        /// <returns>List of <see cref="BoardGameDto"/>. If <paramref name="categoryId"/> is set,
        /// the games are only of this category ID. Otherwise all games are returned.</returns>
        /// <response code="200">Returns the list of board games.</response>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<List<BoardGameDto>>> GetBoardGames(int? categoryId)
        {
            return await _facade.GetBoardGames(this.User, categoryId);
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
    }
}
