using CrmBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CrmBot.DataAccess.Services
{
    public class UserService
    {
        public UserService(IAppUnitOfWorkFactory unitOfWorkFactory)
        {
            uow = unitOfWorkFactory;
        }

        private readonly IAppUnitOfWorkFactory uow;

        public async Task<int> UpsertAsync(User user)
        {
            using (var databaseContext = uow.Create())
            {
                var dbUser = await databaseContext.Users.FirstOrDefaultAsync(u => u.CrmUserId == user.CrmUserId);
                if (dbUser == null)
                {
                    var insertedEntity = await databaseContext.Users.AddAsync(user);
                    await databaseContext.SaveChangesAsync();
                    return insertedEntity.Entity.Id;
                }

                dbUser.FirstName = user.FirstName;
                dbUser.LastName = user.LastName;
                dbUser.TimeZone = user.TimeZone;
                await databaseContext.SaveChangesAsync();
                return dbUser.Id;
            }
        }
    }
}
