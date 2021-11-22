// ReservationItemRepository.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.BoardGames;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class ReservationItemRepository : GenericRepository<ReservationItem, int>, IReservationItemRepository
    {
        public ReservationItemRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ICollection<ReservationItem>> GetItemsInReservation(int reservationId)
        {
            return await Set.Where(i => i.ReservationId == reservationId).ToListAsync();
        }

        public int CountCurrentlyReservingGame(int gameId)
        {
            return Set.Where(i => i.BoardGameId == gameId).Count(i => i.Events.All(e =>
                e.NewState != ReservationItemState.Cancelled && e.NewState != ReservationItemState.Done));
        }

        public async Task UpdateExpiration(int itemId, DateTime newExpiration)
        {
            var item = await this.Get(itemId);
            if (item is not null)
            {
                item.ExpiresOn = newExpiration;
            }
        }

        public async Task<ICollection<ReservationItem>> GetExpiredUnnotified(DateTime? willExpireOn = null)
        {
            var checkingFuture = true;
            if (!willExpireOn.HasValue)
            {
                willExpireOn = DateTime.Now;
                checkingFuture = false;
            }

            var result = Set
                .Where(i => i.ExpiresOn != null)
                .Where(i => i.Events.All(e =>
                    e.NewState != ReservationItemState.Done)) // only Handed-Over or Done will have expiration
                .Where(i => i.ExpiresOn <= willExpireOn);
            if (checkingFuture)
            {
                return await result.Where(i => !i.NotifiedBeforeExpiration).ToListAsync();
            }
            return await result.Where(i => !i.NotifiedOnExpiration).ToListAsync();
        }
    }
}
