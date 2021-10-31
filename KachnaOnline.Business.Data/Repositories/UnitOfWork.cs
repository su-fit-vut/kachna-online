// UnitOfWork.cs
// Author: Ondřej Ondryáš

using System;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;

namespace KachnaOnline.Business.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly AppDbContext _dbContext;
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IBoardGamesRepository BoardGames { get; }
        public IBoardGameCategoriesRepository BoardGamesCategories { get; }

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            this.Users = new UserRepository(dbContext);
            this.Roles = new RoleRepository(dbContext);
            this.BoardGames = new BoardGamesRepository(dbContext);
            this.BoardGamesCategories = new BoardGameCategoriesRepository(dbContext);
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public Task ClearTrackedChanges()
        {
            _dbContext.ChangeTracker.Clear();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
