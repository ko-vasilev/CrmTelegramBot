using System.Threading;
using System.Threading.Tasks;

namespace CrmBot.Internal.Scheduling
{
    /// <summary>
    /// Represents a task which should be executed on a timely manner.
    /// </summary>
    public interface IScheduledTask
    {
        /// <summary>
        /// Task trigger schedule in Cron format.
        /// </summary>
        string Schedule { get; }

        /// <summary>
        /// Executes the task itself.
        /// </summary>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
