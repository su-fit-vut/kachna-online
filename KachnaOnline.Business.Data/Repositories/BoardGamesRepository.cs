// BoardGamesRepository.cs
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
    public class BoardGamesRepository : GenericRepository<BoardGame, int>, IBoardGamesRepository
    {
        public BoardGamesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<BoardGame> GetWithCategory(int boardGameId)
        {
            return Set.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == boardGameId);
        }

        public async Task<ICollection<BoardGame>> GetFilteredGames(int? categoryId, int? players, bool? available,
            bool? visible)
        {
            var result = Set.AsQueryable();
            if (categoryId is not null)
                result = result.Where(b => b.CategoryId == categoryId);

            if (players is not null)
            {
                result = result.Where(b =>
                    b.PlayersMin != null && b.PlayersMin <= players && b.PlayersMax != null && b.PlayersMax >= players);
            }

            if (available is not null)
            {
                result = result.Where(b => (b.InStock - b.Unavailable - b.ReservationItems.Count(i =>
                                                i.Events.All(e =>
                                                    e.NewState != ReservationItemState.Cancelled &&
                                                    e.NewState != ReservationItemState.Done)) >
                                            0) == available);
            }

            if (visible is not null)
                result = result.Where(b => b.Visible == visible);

            return await result.Include(b => b.Category).ToListAsync();
        }
    }
}
