using System;
using System.Collections.Generic;
using KachnaOnline.Data.Entities.ClubStates;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IRepeatingStatesRepository : IGenericRepository<RepeatingState, int>
    {
        IAsyncEnumerable<RepeatingState> All(DateTime? effectiveFrom, DateTime? effectiveTo);
        IAsyncEnumerable<RepeatingState> EffectiveAt(DateTime at);
    }
}
