// IUnitOfWork.cs
// Author: Ondřej Ondryáš

using System.Threading.Tasks;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IBoardGameRepository BoardGames { get; }
        IBoardGameCategoryRepository BoardGameCategories { get; }
        Task SaveChanges();
        Task ClearTrackedChanges();
    }
}
