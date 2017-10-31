using CrmBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CrmBot.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<CrmUserKey> UserKeys { get; set; }

        public DbSet<TelegramChat> TelegramChats { get; set; }
    }
}
