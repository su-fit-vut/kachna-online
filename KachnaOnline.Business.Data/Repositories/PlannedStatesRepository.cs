// PlannedStatesRepository.cs
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
    public class PlannedStatesRepository : GenericRepository<PlannedState, int>, IPlannedStatesRepository
    {
        public PlannedStatesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PlannedState> GetWithNext(int id)
        {
            return await Set
                .Where(s => s.Id == id)
                .Include(s => s.NextPlannedState)
                .FirstOrDefaultAsync();
        }

        public async Task<PlannedState> GetCurrent(DateTime? atTime = null, bool includeEndMinute = false,
            bool includeNext = false)
        {
            var actualAtTime = atTime ?? DateTime.Now;
            IQueryable<PlannedState> query;

            if (includeEndMinute)
            {
                query = Set
                    .Where(s => s.Start <= actualAtTime && s.PlannedEnd >= actualAtTime && s.Ended == null);
            }
            else
            {
                query = Set
                    .Where(s => s.Start <= actualAtTime && s.PlannedEnd > actualAtTime && s.Ended == null);
            }

            if (includeNext)
            {
                query = query.Include(q => q.NextPlannedState);
            }

            return await query
                .OrderByDescending(s => s.Start)
                .FirstOrDefaultAsync();
        }

        public async Task<PlannedState> GetNearest(StateType? ofType, DateTime? after, bool includeNext)
        {
            var currentState = await this.GetCurrent(includeNext: includeNext);
            if (currentState is {NextPlannedStateId: not null})
            {
                var nextState = await this.Get(currentState.NextPlannedStateId.Value);
                if ((!ofType.HasValue || nextState.State == ofType.Value) &&
                    (!after.HasValue || nextState.Start >= after.Value))
                {
                    return nextState;
                }
            }

            var afterDate = after ?? DateTime.Now;
            IQueryable<PlannedState> query = Set
                .Where(s => !ofType.HasValue || s.State == ofType.Value)
                .Where(s => s.Start >= afterDate)
                .OrderBy(s => s.Start);

            if (includeNext)
            {
                query = query.Include(s => s.NextPlannedState);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<PlannedState> GetLastEnded()
        {
            return await Set
                .Where(s => s.Ended.HasValue && s.Ended.Value <= DateTime.Now)
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

        public IAsyncEnumerable<PlannedState> GetForRepeatingState(int repeatingStateId, DateTime? onlyAfter,
            bool includeNextStates)
        {
            var query = Set
                .Where(s => s.RepeatingStateId == repeatingStateId);

            if (onlyAfter.HasValue)
                query = query.Where(s => s.Start >= onlyAfter.Value);

            if (includeNextStates)
                query = query.Include(s => s.NextPlannedState);

            return query.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<PlannedState> GetPastNotEnded()
        {
            return Set
                .Where(s => s.PlannedEnd < DateTime.Now && !s.Ended.HasValue)
                .Include(s => s.NextPlannedState)
                .AsAsyncEnumerable();
        }
    }
}
