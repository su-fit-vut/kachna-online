// IReservationRepository.cs
// Author: František Nečas

using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Data.Entities.BoardGames;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IReservationRepository : IGenericRepository<Reservation, int>
    {
        Task<ICollection<Reservation>> GetByUserId(int userId);

        Task<ICollection<Reservation>> GetByAssignedUserId(int? userId);
    }
}
