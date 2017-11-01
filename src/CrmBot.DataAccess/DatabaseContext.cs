using CrmBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrmBot.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Chat)
                .WithOne(ch => ch.User)
                .HasForeignKey<TelegramChat>(ch => ch.UserId);

            modelBuilder.Entity<TelegramChat>(ch =>
            {
                ch.Property(chat => chat.SecureKey)
                    .HasDefaultValueSql("NEWID()");
            });
        }

        public DbSet<User> Users { get; set; }

        public DbSet<TelegramChat> TelegramChats { get; set; }

        public override int SaveChanges()
        {
            UpdateAutomaticValues();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAutomaticValues();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Sets the updated date for all necessary entities.
        /// </summary>
        private void UpdateAutomaticValues()
        {
            ChangeTracker.DetectChanges();
            var now = DateTime.UtcNow;

            foreach (var item in ChangeTracker.Entries<TelegramChat>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                item.Property(ch => ch.UpdatedDate).CurrentValue = now;
            }
        }
    }
}
