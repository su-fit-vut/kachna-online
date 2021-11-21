// UserRoleRepository.cs
// Author: František Nečas

using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using KachnaOnline.Data.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Business.Data.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        protected AppDbContext Context;
        protected DbSet<UserRole> Set;

        public UserRoleRepository(AppDbContext dbContext)
        {
            Context = dbContext;
            Set = dbContext.Set<UserRole>();
        }
        
        public async Task<UserRole> Get(int userId, int roleId)
        {
            return await Set.FindAsync(userId, roleId);
        }

        public Task Delete(UserRole assignment)
        {
            Set.Remove(assignment);
            return Task.CompletedTask;
        }

        public Task Add(UserRole assignment)
        {
            Set.Add(assignment);
            return Task.CompletedTask;
        }
    }
}
