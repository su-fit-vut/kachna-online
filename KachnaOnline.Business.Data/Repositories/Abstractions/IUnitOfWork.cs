// IUnitOfWork.cs
// Author: Ondřej Ondryáš

using System.Threading.Tasks;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IBoardGamesRepository BoardGames { get; }
        IBoardGameCategoriesRepository BoardGamesCategories { get; }
        IReservationRepository Reservations { get; }
        IReservationItemRepository ReservationItems { get; }
        IReservationItemEventRepository ReservationItemEvents { get; }
        Task SaveChanges();
        Task ClearTrackedChanges();
    }
}
