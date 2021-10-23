// EventsRepository.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.Events;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class EventsRepository : GenericRepository<Event, int>, IEventsRepository
    {
        public EventsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public IAsyncEnumerable<Event> GetCurrent(DateTime? atTime = null)
        {
            var actualAtTime = atTime ?? DateTime.Now;

            return Set
                .Where(e => e.From <= actualAtTime && e.To >= actualAtTime)
                .AsAsyncEnumerable();
        }

        public IEnumerable<Event> GetNearest(DateTime? after = null)
        {
            var afterDate = after ?? DateTime.Now;

            return Set.Where(e => e.From >= DateTime.Now)
                .AsEnumerable()
                .GroupBy(e => e.From)
                .OrderBy(g => g.Key)
                .FirstOrDefault();
        }

        public IAsyncEnumerable<Event> GetStartingBetween(DateTime from, DateTime to)
        {
            return Set
                .Where(e => e.From >= from && e.From <= to)
                .AsAsyncEnumerable();
        }
    }
}
