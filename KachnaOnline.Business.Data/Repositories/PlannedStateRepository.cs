// PlannedStateRepository.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.ClubStates;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class PlannedStateRepository : GenericRepository<PlannedState, int>, IPlannedStateRepository
    {
        public PlannedStateRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PlannedState> GetCurrent(DateTime? atTime = null)
        {
            var actualAtTime = atTime ?? DateTime.Now;

            return await Set
                .Where(s => s.Start <= actualAtTime && s.PlannedEnd > actualAtTime && s.Ended == null)
                .OrderByDescending(s => s.Start)
                .FirstOrDefaultAsync();
        }

        public async Task<PlannedState> GetNearest(StateType? ofType, DateTime? after)
        {
            var currentState = await this.GetCurrent();
            if (currentState is { NextPlannedStateId: not null })
            {
                var nextState = await this.Get(currentState.NextPlannedStateId.Value);
                if ((!ofType.HasValue || nextState.State == ofType.Value) &&
                    (!after.HasValue || nextState.Start >= after.Value))
                {
                    return nextState;
                }
            }

            var afterDate = after ?? DateTime.Now;
            return await Set
                .Where(s => !ofType.HasValue || s.State == ofType.Value)
                .Where(s => s.Start >= afterDate)
                .OrderBy(s => s.Start)
                .FirstOrDefaultAsync();
        }

        public async Task<PlannedState> GetLast()
        {
            return await Set
                .Where(s => s.Ended.HasValue && s.Ended.Value < DateTime.Now)
                .OrderByDescending(s => s.Ended)
                .FirstOrDefaultAsync();
        }

        public async Task<PlannedState> GetPreviousFor(int stateId)
        {
            return await Set
                .Where(s => s.NextPlannedStateId == stateId)
                .OrderByDescending(s => s.PlannedEnd)
                .FirstOrDefaultAsync();
        }

        public IAsyncEnumerable<PlannedState> GetStartingBetween(DateTime from, DateTime to, bool includeNextStates)
        {
            var query = Set
                .Where(s => s.Start >= from && s.Start <= to);

            if (includeNextStates)
                query = query.Include(s => s.NextPlannedState);

            return query.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<PlannedState> GetForRepeatingState(int repeatingStateId, bool futureOnly,
            bool includeNextStates)
        {
            var query = Set
                .Where(s => s.RepeatingStateId == repeatingStateId);

            if (futureOnly)
                query = query.Where(s => s.Start > DateTime.Now);

            if (includeNextStates)
                query = query.Include(s => s.NextPlannedState);

            return query.AsAsyncEnumerable();
        }
    }
}
