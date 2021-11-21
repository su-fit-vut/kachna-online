// IUserRoleRepository.cs
// Author: František Nečas

using System.Threading.Tasks;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IUserRoleRepository
    {
        Task<UserRole> Get(int userId, int roleId);
        Task Delete(UserRole assignment);
        Task Add(UserRole assignment);
    }
}
