using System.Threading.Tasks;
using KachnaOnline.Data.Entities.Users;

namespace KachnaOnline.Business.Data.Repositories.Abstractions
{
    public interface IRoleRepository : IGenericRepository<Role, int>
    {
        Task<Role> GetByName(string name);
    }
}
