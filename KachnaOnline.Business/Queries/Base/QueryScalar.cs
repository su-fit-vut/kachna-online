using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Queries.Base
{
    public abstract class QueryScalar<TResult, TDbContext> where TDbContext : DbContext
    {
        protected readonly TDbContext Context;

        protected QueryScalar(TDbContext context)
        {
            Context = context;
        }

        public abstract Task<TResult> GetAsync();
    }

    public abstract class QueryScalar<TResult, TPar1, TDbContext> where TDbContext : DbContext
    {
        protected readonly TDbContext Context;

        protected QueryScalar(TDbContext context)
        {
            Context = context;
        }

        public abstract Task<TResult> GetAsync(TPar1 par1);
    }
}
