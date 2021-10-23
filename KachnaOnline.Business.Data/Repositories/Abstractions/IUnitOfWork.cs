// IUnitOfWork.cs
// Author: Ondřej Ondryáš, David Chocholatý

using System.Threading.Tasks;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IBoardGamesRepository BoardGames { get; }
        IBoardGameCategoriesRepository BoardGamesCategories { get; }
        IEventsRepository Events { get; }

        Task SaveChanges();
        Task ClearTrackedChanges();
    }
}
