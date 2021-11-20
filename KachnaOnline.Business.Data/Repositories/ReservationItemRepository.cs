// ReservationItemRepository.cs
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
    public class ReservationItemRepository : GenericRepository<ReservationItem, int>, IReservationItemRepository
    {
        public ReservationItemRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ICollection<ReservationItem>> ItemsInReservation(int reservationId)
        {
            return await Set.Where(i => i.ReservationId == reservationId).ToListAsync();
        }
    }
}
