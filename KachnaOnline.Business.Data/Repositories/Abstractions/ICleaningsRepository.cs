using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Data.Entities.Cleanings;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface ICleaningsRepository : IGenericRepository<Cleaning, int>
    {
        IAsyncEnumerable<Cleaning> GetCurrent(DateTime? at = null);
        IAsyncEnumerable<Cleaning> GetUnfinished();
        IAsyncEnumerable<Cleaning> GetNearest(DateTime? after = null);
        IAsyncEnumerable<Cleaning> GetStartingBetween(DateTime from, DateTime to);
    }
}
