// BoardGameRepository.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.BoardGames;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class BoardGameRepository : GenericRepository<BoardGame, int>, IBoardGameRepository
    {
        public BoardGameRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<BoardGame> GetWithCategory(int boardGameId)
        {
            return Set.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == boardGameId);
        }

        public IAsyncEnumerable<BoardGame> GetFilteredGames(int? categoryId, int? players, bool? available,
            bool? visible)
        {
            var result = Set.AsQueryable();
            if (categoryId is not null)
            {
                result = result.Where(b => b.CategoryId == categoryId);
            }

            if (players is not null)
            {
                result = result.Where(b =>
                    (b.PlayersMin != null && b.PlayersMin <= players) &&
                    (b.PlayersMax != null && b.PlayersMax >= players));
            }

            if (available is not null)
            {
                // TODO: reservations
                result = result.Where(b => (b.InStock - b.Unavailable) > 0 == available);
            }

            if (visible is not null)
            {
                result = result.Where(b => b.Visible == visible);
            }

            return result.Include(b => b.Category).AsAsyncEnumerable();
        }
    }
}
