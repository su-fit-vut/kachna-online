// PushSubscriptionsRepository.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.PushSubscriptions;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class PushSubscriptionsRepository : GenericRepository<PushSubscription, string>, IPushSubscriptionsRepository
    {
        public PushSubscriptionsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PushSubscription> GetWithKeys(string endpoint)
        {
            return await Set.Include(s => s.Keys).FirstOrDefaultAsync(s => s.Endpoint == endpoint);
        }

        public IAsyncEnumerable<PushSubscription> GetSubscribedToStateChanges()
        {
            return Set.Where(s => s.StateChangesEnabled).Include(s => s.Keys).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<PushSubscription> GetUserBoardGamesSubscription(int userId)
        {
            return Set.Where(s => s.MadeById == userId).Where(s => s.BoardGamesEnabled).Include(s => s.Keys)
                .AsAsyncEnumerable();
        }
    }
}
