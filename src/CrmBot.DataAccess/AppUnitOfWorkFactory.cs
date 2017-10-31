using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CrmBot.DataAccess
{
    public class AppUnitOfWorkFactory : IAppUnitOfWorkFactory
    {
        readonly DbContextOptions<DatabaseContext> dbOptions;

        public AppUnitOfWorkFactory(DbContextOptions<DatabaseContext> dbOptions)
        {
            this.dbOptions = dbOptions;
        }

        /// <inheritdoc />
        public DatabaseContext Create(IsolationLevel isolationLevel)
        {
            return new DatabaseContext(dbOptions);
        }

        /// <inheritdoc />
        public DatabaseContext Create()
        {
            return new DatabaseContext(dbOptions);
        }
    }
}
