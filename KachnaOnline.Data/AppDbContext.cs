// AppDbContext.cs
// Author: Ondřej Ondryáš

using KachnaOnline.Data.Entities.BoardGames;
using KachnaOnline.Data.Entities.ClubStates;
using KachnaOnline.Data.Entities.Events;
using KachnaOnline.Data.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace KachnaOnline.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<PlannedState> PlannedStates { get; set; }
        public DbSet<RepeatingState> RepeatingStates { get; set; }
        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<Category> BoardGameCategories { get; set; }
        public DbSet<Reservation> BoardGameReservations { get; set; }
        public DbSet<ReservationItem> BoardGameReservationItems { get; set; }

        public AppDbContext() : base()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Users
            builder.Entity<UserRole>()
                .HasOne(e => e.User)
                .WithMany(e => e.Roles)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserRole>()
                .HasOne(e => e.Role)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserRole>()
                .HasOne(e => e.AssignedByUser)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UserRole>()
                .HasKey(e => new { e.UserId, e.RoleId });

            // Club states
            builder.Entity<RepeatingState>()
                .HasMany(e => e.LinkedPlannedStates)
                .WithOne(e => e.RepeatingState)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RepeatingState>()
                .HasOne(e => e.MadeBy)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PlannedState>()
                .HasOne(e => e.NextPlannedState)
                .WithOne();

            builder.Entity<PlannedState>()
                .HasOne(e => e.AssociatedEvent)
                .WithMany(e => e.LinkedPlannedStates)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<PlannedState>()
                .HasOne(e => e.MadeBy)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PlannedState>()
                .HasOne(e => e.ClosedBy)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Events
            builder.Entity<Event>()
                .HasOne(e => e.MadeBy)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Board games
            builder.Entity<BoardGame>()
                .HasOne(e => e.Category)
                .WithMany(e => e.Games)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BoardGame>()
                .HasOne(e => e.Owner)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Reservation>()
                .HasMany(e => e.Items)
                .WithOne(e => e.Reservation)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Reservation>()
                .HasOne(e => e.MadeBy)
                .WithMany(e => e.Reservations)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReservationItem>()
                .HasMany(e => e.Events)
                .WithOne(e => e.ReservationItem)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReservationItem>()
                .HasOne(e => e.BoardGame)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReservationItemEvent>()
                .HasOne(e => e.MadeBy)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReservationItemEvent>()
                .HasKey(e => new { e.ReservationItemId, e.MadeOn });

            base.OnModelCreating(builder);
        }
    }
}
