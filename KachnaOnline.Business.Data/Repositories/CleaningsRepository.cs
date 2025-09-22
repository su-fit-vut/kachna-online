using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.Cleanings;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class CleaningsRepository : GenericRepository<Cleaning, int>, ICleaningsRepository
    {
        public CleaningsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Cleaning> Get(int key)
        {
            return await Set.FirstOrDefaultAsync(e => e.Id == key);
        }

        public IAsyncEnumerable<Cleaning> GetCurrent(DateTime? atTime = null)
        {
            var actualAtTime = atTime ?? DateTime.Now;

            return Set
                .Where(e => e.From <= actualAtTime && e.To >= actualAtTime)
                .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<Cleaning> GetUnfinished()
        {
            return Set
                .Where(e => e.Finished == false)
                .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<Cleaning> GetNearest(DateTime? after = null)
        {
            var afterDate = after ?? DateTime.Now;

            var cleaningEntity = Set.Where(e => e.From > afterDate).OrderBy(e => e.From).FirstOrDefaultAsync();

            if (cleaningEntity is not null)
                return Set.Where(e => e.From == cleaningEntity.Result.From).AsAsyncEnumerable();

            return Enumerable.Empty<Cleaning>() as IAsyncEnumerable<Cleaning>;
        }

        public IAsyncEnumerable<Cleaning> GetStartingBetween(DateTime from, DateTime to)
        {
            return Set
                .Where(e => e.From >= from && e.From <= to)
                .AsAsyncEnumerable();
        }
    }
}
