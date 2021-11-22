// RepeatingStatesRepository.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Linq;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.ClubStates;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class RepeatingStatesRepository : GenericRepository<RepeatingState, int>, IRepeatingStatesRepository
    {
        public RepeatingStatesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public IAsyncEnumerable<RepeatingState> All(DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            var query = Set.AsQueryable();

            if (effectiveFrom.HasValue)
                query = query.Where(rs => rs.EffectiveFrom <= effectiveFrom.Value);

            if (effectiveTo.HasValue)
                query = query.Where(rs => rs.EffectiveTo >= effectiveTo.Value);

            return query.OrderBy(rs => rs.EffectiveFrom).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<RepeatingState> EffectiveAt(DateTime at)
        {
            return Set
                .Where(rs => rs.EffectiveFrom <= at && rs.EffectiveTo >= at)
                .OrderBy(rs => rs.EffectiveFrom)
                .AsAsyncEnumerable();
        }
    }
}
