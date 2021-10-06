// IUserRepository.cs
// Author: Ondřej Ondryáš

using System.Threading.Tasks;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IUserRepository : IGenericRepository<User, int>
    {
        Task<User> GetWithRoles(int id);
    }
}
