// QueryCommited.cs
// Author: Ondřej Ondryáš

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KachnaOnline.Business.Queries.Base
{
    public abstract class QueryCommited<TDbContext> where TDbContext : DbContext
    {
        protected readonly TDbContext Context;

        protected QueryCommited(TDbContext context)
        {
            Context = context;
        }

        public virtual async Task CommitAsync(IDbContextTransaction transaction = null)
        {
            await this.MakeQuery(transaction).ConfigureAwait(false);
            await Context.SaveChangesAsync(true).ConfigureAwait(false);
        }

        public abstract Task MakeQuery(IDbContextTransaction transaction = null);
    }

    public abstract class QueryCommited<TReturnValue, TDbContext> where TDbContext : DbContext
    {
        protected readonly TDbContext Context;

        protected QueryCommited(TDbContext context)
        {
            Context = context;
        }

        public virtual async Task<TReturnValue> CommitAsync(IDbContextTransaction transaction = null)
        {
            var retValue = await this.MakeQuery(transaction).ConfigureAwait(false);
            await Context.SaveChangesAsync(true).ConfigureAwait(false);
            return retValue;
        }

        public abstract Task<TReturnValue> MakeQuery(IDbContextTransaction transaction = null);
    }
}
