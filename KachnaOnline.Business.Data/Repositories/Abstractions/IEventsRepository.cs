// IEventsRepository.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Data.Entities.Events;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IEventsRepository : IGenericRepository<Event, int>
    {
        IAsyncEnumerable<Event> GetCurrent(DateTime? at = null);
        IAsyncEnumerable<Event> GetNearest(DateTime? after = null);
        IAsyncEnumerable<Event> GetStartingBetween(DateTime from, DateTime to);
    }
}
