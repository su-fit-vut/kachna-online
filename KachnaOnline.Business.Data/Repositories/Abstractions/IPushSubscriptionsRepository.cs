// IPushSubscriptionsRepository.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Data.Entities.PushSubscriptions;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IPushSubscriptionsRepository : IGenericRepository<PushSubscription, string>
    {
        public Task<PushSubscription> GetWithKeys(string endpoint);

        public IAsyncEnumerable<PushSubscription> GetSubscribedToStateChanges();

        public IAsyncEnumerable<PushSubscription> GetUserBoardGamesSubscription(int userId);
    }
}
