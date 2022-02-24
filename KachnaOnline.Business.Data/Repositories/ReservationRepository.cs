using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.BoardGames;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class ReservationRepository : GenericRepository<Reservation, int>, IReservationRepository
    {
        public ReservationRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ICollection<Reservation>> GetByUserId(int userId)
        {
            return await Set.Where(r => r.MadeById == userId).ToListAsync();
        }

        public async Task<ICollection<Reservation>> GetByAssignedUserId(int? userId)
        {
            var result = Set.AsQueryable();
            if (userId.HasValue)
            {
                result = result.Where(r => r.Items.Any(i =>
                    i.Events.Any(e => e.Type == ReservationEventType.Assigned && e.MadeById == userId.Value)));
            }

            return await result.ToListAsync();
        }
    }
}
