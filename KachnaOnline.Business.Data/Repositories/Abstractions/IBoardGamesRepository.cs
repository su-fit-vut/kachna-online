// IBoardGamesRepository.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Data.Entities.BoardGames;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IBoardGamesRepository : IGenericRepository<BoardGame, int>
    {
        Task<ICollection<BoardGame>> GetFilteredGames(int? categoryId, int? players, bool? available, bool? visible);

        Task<BoardGame> GetWithCategory(int boardGameId);
    }
}
