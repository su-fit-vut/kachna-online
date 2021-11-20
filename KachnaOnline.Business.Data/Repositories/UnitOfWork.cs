// UnitOfWork.cs
// Author: Ondřej Ondryáš

using System;
using System.Threading.Tasks;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace KachnaOnline.Business.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly AppDbContext _dbContext;
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IBoardGamesRepository BoardGames { get; }
        public IBoardGameCategoriesRepository BoardGamesCategories { get; }
        public IReservationRepository Reservations { get; }
        public IReservationItemRepository ReservationItems { get; }
        public IReservationItemEventRepository ReservationItemEvents { get; }

        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            this.Users = new UserRepository(dbContext);
            this.Roles = new RoleRepository(dbContext);
            this.BoardGames = new BoardGamesRepository(dbContext);
            this.BoardGamesCategories = new BoardGameCategoriesRepository(dbContext);
            this.Reservations = new ReservationRepository(dbContext);
            this.ReservationItems = new ReservationItemRepository(dbContext);
            this.ReservationItemEvents = new ReservationItemEventRepository(dbContext);
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

        public async Task BeginTransaction()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransaction()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
