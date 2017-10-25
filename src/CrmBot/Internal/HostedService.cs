using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace CrmBot.Internal
{
    /// <summary>
    /// Provides generic functionality to handle hosted services lifetime cycle.
    /// </summary>
    public abstract class HostedService : IHostedService
    {
        private Task executingTask;

        private CancellationTokenSource cancellationTokenSource;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation.
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            executingTask = ExecuteAsync(cancellationTokenSource.Token);

            // If the task is completed then return it, otherwise it's running
            return executingTask.IsCompleted ? executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            cancellationTokenSource.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(executingTask, Task.Delay(-1, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Executes associated action.
        /// This method should be overridden and contain the main task logic.
        /// </summary>
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
