// IUnitOfWork.cs
// Author: Ondřej Ondryáš, David Chocholatý

using System.Threading.Tasks;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IPlannedStateRepository PlannedStates { get; }
        IBoardGamesRepository BoardGames { get; }
        IBoardGameCategoriesRepository BoardGamesCategories { get; }
        IReservationRepository Reservations { get; }
        IReservationItemRepository ReservationItems { get; }
        IReservationItemEventRepository ReservationItemEvents { get; }
        IEventsRepository Events { get; }

        Task SaveChanges();
        Task ClearTrackedChanges();
        Task BeginTransaction();
        Task CommitTransaction();
        Task RollbackTransaction();
    }
}
