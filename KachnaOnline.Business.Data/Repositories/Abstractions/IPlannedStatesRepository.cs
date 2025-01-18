using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Data.Entities.ClubStates;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IPlannedStatesRepository : IGenericRepository<PlannedState, int>
    {
        Task<PlannedState> GetWithNext(int id);
        Task<PlannedState> GetCurrent(DateTime? at = null, bool includeEndMinute = false, bool includeNext = false);
        Task<PlannedState> GetNearest(StateType? ofType = null, DateTime? after = null, bool includeNext = false);
        Task<PlannedState> GetLastEnded();
        Task<PlannedState> GetPreviousFor(int stateId);
        IAsyncEnumerable<PlannedState> GetStartingBetween(DateTime from, DateTime to, bool includeNextStates = false);
        Task<List<PlannedState>> GetAllCollidingStates(DateTime start, DateTime end);
        IAsyncEnumerable<PlannedState> GetConflictingStatesForEvent(DateTime from, DateTime to);
        IAsyncEnumerable<PlannedState> GetForRepeatingState(int repeatingStateId, DateTime? onlyAfter = null,
            bool includeNextStates = false);
        IAsyncEnumerable<PlannedState> GetPastNotEnded();
    }
}
