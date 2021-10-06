// RoleRepository.cs
// Author: Ondřej Ondryáš

using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class RoleRepository : GenericRepository<Role, int>, IRoleRepository
    {
        public RoleRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<Role> GetByName(string name)
        {
            return Set.Where(r => r.Name == name).FirstOrDefaultAsync();
        }
    }
}
