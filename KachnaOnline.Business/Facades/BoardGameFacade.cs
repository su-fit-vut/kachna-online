// BoardGameFacade.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KachnaOnline.Business.Models.BoardGames;
using KachnaOnline.Business.Services.Abstractions;
using KachnaOnline.Dto.BoardGames;
using KachnaOnline.Business.Constants;

namespace KachnaOnline.Business.Facades
{
    public class BoardGameFacade
    {
        private readonly IMapper _mapper;
        private readonly IBoardGameService _boardGameService;

        public BoardGameFacade(IBoardGameService boardGameService, IMapper mapper)
        {
            _boardGameService = boardGameService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all categories of board games.
        /// </summary>
        /// <returns>A list of <see cref="CategoryDto"/>.</returns>
        public async Task<List<CategoryDto>> GetCategories()
        {
            var categories = await _boardGameService.GetBoardGameCategories();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        /// <summary>
        /// Sets <see cref="BoardGameDto"/> attributes to null if the given user does not have the right to see them.
        /// </summary>
        /// <param name="user">User to check the authorization for. All data is visible to board game manager.</param>
        /// <param name="game">Game to hide details of.</param>
        private void HidePrivateBoardGameAttributes(ClaimsPrincipal user, BoardGameDto game)
        {
            if (!user.IsInRole(RoleConstants.BoardGamesManager))
            {
                game.NoteInternal = null;
                game.OwnerId = null;
                game.DefaultReservationTime = null;
                game.Visible = null;
            }
        }
        
        /// <summary>
        /// Returns the list of board games.
        /// </summary>
        /// <param name="user">User requesting the data.</param>
        /// <param name="categoryId">If set, returns only the board games of this category ID.</param>
        /// <returns>A list of <see cref="BoardGameDto"/>. The attributes of each <see cref="BoardGameDto"/>
        /// depend on the user requesting the data.</returns>
        public async Task<List<BoardGameDto>> GetBoardGames(ClaimsPrincipal user, int? categoryId)
        {
            ICollection<BoardGame> games;
            if (categoryId is null)
            {
                games = await _boardGameService.GetBoardGames();
            }
            else
            {
                games = await _boardGameService.GetBoardGamesInCategory(categoryId.Value);
            }
            var result = new List<BoardGameDto>();
            foreach (var game in games)
            {
                var dto = _mapper.Map<BoardGameDto>(game);
                HidePrivateBoardGameAttributes(user, dto);
                result.Add(dto);
            }
            return result;
        }
    }
}
