// GenericRepository.cs
// Author: Ondřej Ondryáš

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public abstract class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        protected AppDbContext Context;
        protected DbSet<TEntity> Set;

        public GenericRepository(AppDbContext dbContext)
        {
            Context = dbContext;
            Set = dbContext.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> All()
        {
            return Set;
        }

        public virtual async Task<TEntity> Get(TKey key)
        {
            return await Set.FindAsync(key);
        }

        public virtual Task Add(TEntity entity)
        {
            Set.Add(entity);
            return Task.CompletedTask;
        }

        public virtual async Task Delete(TKey key)
        {
            var entity = await this.Get(key);
            if (entity != null)
            {
                Set.Remove(entity);
            }
        }

        public virtual Task Delete(TEntity entity)
        {
            Set.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate);
        }
    }
}
