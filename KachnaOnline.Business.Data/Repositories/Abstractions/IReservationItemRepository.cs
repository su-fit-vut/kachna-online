// IReservationItemRepository.cs
// Author: František Nečas

using KachnaOnline.Data.Entities.BoardGames;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IReservationItemRepository : IGenericRepository<ReservationItem, int>
    {
    }
}
