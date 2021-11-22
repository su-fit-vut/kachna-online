// IReservationItemRepository.cs
// Author: František Nečas

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Data.Entities.BoardGames;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IReservationItemRepository : IGenericRepository<ReservationItem, int>
    {
        Task<ICollection<ReservationItem>> GetItemsInReservation(int reservationId);
        int CountCurrentlyReservingGame(int gameId);
        Task UpdateExpiration(int itemId, DateTime newExpiration);
        Task<ICollection<ReservationItem>> GetExpiredUnnotified(DateTime? willExpireOn = null);
    }
}
