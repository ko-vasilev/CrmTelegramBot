using CrmBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

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
        }

        public DbSet<User> Users { get; set; }

        public DbSet<TelegramChat> TelegramChats { get; set; }
    }
}
