// UserRepository.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<User> GetWithRoles(int key)
        {
            return Set.Where(e => e.Id == key)
                .Include(e => e.Roles)
                .ThenInclude(e => e.Role)
                .Include(e => e.Roles)
                .ThenInclude(e => e.AssignedByUser)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetFiltered(string filter)
        {
            filter = filter.ToLower(CultureInfo.GetCultureInfo("cs-CZ"));

            return await Set
                .Where(e => e.Name.ToLower().Contains(filter)
                            || e.Nickname.ToLower().Contains(filter)
                            || e.Email.ToLower().StartsWith(filter))
                .ToListAsync();
        }
    }
}
