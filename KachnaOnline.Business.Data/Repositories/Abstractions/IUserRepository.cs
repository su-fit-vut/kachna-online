using System.Collections.Generic;
using System.Threading.Tasks;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IUserRepository : IGenericRepository<User, int>
    {
        Task<List<User>> GetUsers(List<int> ids);
        Task<User> GetWithRoles(int id);
        Task<List<User>> GetFiltered(string filter);
    }
}
