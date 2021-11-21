// ReservationItemEventRepository.cs
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
    public class ReservationItemEventRepository : IReservationItemEventRepository
    {
        protected AppDbContext Context;
        protected DbSet<ReservationItemEvent> Set;
        public ReservationItemEventRepository(AppDbContext dbContext)
        {
            Context = dbContext;
            Set = dbContext.Set<ReservationItemEvent>();
        }

        public async Task<ICollection<ReservationItemEvent>> GetByItemIdChronologically(int itemId)
        {
            return await Set.Where(e => e.ReservationItemId == itemId).OrderBy(e => e.MadeOn).ToListAsync();
        }

        public Task<ReservationItemEvent> GetLatestEvent(int itemId)
        {
            return Set.Where(e => e.ReservationItemId == itemId).OrderByDescending(e => e.MadeOn).Take(1).SingleAsync();
        }

        public Task Add(ReservationItemEvent entity)
        {
            Set.Add(entity);
            return Task.CompletedTask;
        }
    }
}
