// EventsRepository.cs
// Author: David Chocholatý

using System;
using System.Collections.Generic;
using System.Linq;
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

        public override async Task<Event> Get(int key)
        {
            return await Set.Include(e => e.LinkedPlannedStates).FirstOrDefaultAsync(e => e.Id == key);
        }

        public IAsyncEnumerable<Event> GetCurrent(DateTime? atTime = null)
        {
            var actualAtTime = atTime ?? DateTime.Now;

            return Set
                .Where(e => e.From <= actualAtTime && e.To >= actualAtTime)
                .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<Event> GetNearest(DateTime? after = null)
        {
            var afterDate = after ?? DateTime.Now;

            var eventEntity = Set.Where(e => e.From > afterDate).OrderBy(e => e.From).FirstOrDefaultAsync();

            if (eventEntity is not null)
                return Set.Where(e => e.From == eventEntity.Result.From).AsAsyncEnumerable();

            return Enumerable.Empty<Event>() as IAsyncEnumerable<Event>;
        }

        public IAsyncEnumerable<Event> GetStartingBetween(DateTime from, DateTime to)
        {
            return Set
                .Where(e => e.From >= from && e.From <= to)
                .AsAsyncEnumerable();
        }

        public async Task<Event> GetWithLinkedStates(int eventId)
        {
            return await Set.Include(e => e.LinkedPlannedStates).FirstOrDefaultAsync(e => e.Id == eventId);
        }
    }
}
