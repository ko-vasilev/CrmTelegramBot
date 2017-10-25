using System;
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
        /// How often should task be executed.
        /// </summary>
        TimeSpan RepeatFrequency { get; }

        /// <summary>
        /// Executes the task itself.
        /// </summary>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
