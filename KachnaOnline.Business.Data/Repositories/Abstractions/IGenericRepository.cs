// IGenericRepository.cs
// Author: Ondřej Ondryáš

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IGenericRepository<TEntity, in TKey> where TEntity : class
    {
        IQueryable<TEntity> All();
        Task<TEntity> Get(TKey key);
        Task Add(TEntity entity);
        Task Delete(TKey key);
        Task Delete(TEntity entity);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}
