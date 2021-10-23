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

        private IUserRepository _users;
        public IUserRepository Users => _users ??= new UserRepository(_dbContext);

        private IRoleRepository _roles;
        public IRoleRepository Roles => _roles ??= new RoleRepository(_dbContext);

        private IPlannedStateRepository _plannedStates;
        public IPlannedStateRepository PlannedStates => _plannedStates ??= new PlannedStateRepository(_dbContext);

        private IBoardGamesRepository _boardGames;
        public IBoardGamesRepository BoardGames => _boardGames ??= new BoardGamesRepository(_dbContext);

        private IBoardGameCategoriesRepository _boardGameCategories;
        public IBoardGameCategoriesRepository BoardGamesCategories => _boardGameCategories ??= new BoardGameCategoriesRepository(_dbContext);

        private IEventsRepository _events;
        public IEventsRepository Events => _events ??= new EventsRepository(_dbContext);

        public IReservationRepository _reservations;
        public IReservationRepository Reservations => _reservations ??= new ReservationRepository(_dbContext);

        public IReservationItemRepository _reservationItems;
        public IReservationItemRepository ReservationItems =>
            _reservationItems ??= new ReservationItemRepository(_dbContext);

        public IReservationItemEventRepository _reservationItemEvents;
        public IReservationItemEventRepository ReservationItemEvents =>
            _reservationItemEvents ??= new ReservationItemEventRepository(_dbContext);

        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
