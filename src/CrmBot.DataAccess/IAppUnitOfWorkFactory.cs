using Saritasa.Tools.Domain;

namespace CrmBot.DataAccess
{
    public interface IAppUnitOfWorkFactory: IUnitOfWorkFactory<DatabaseContext>
    {
    }
}
