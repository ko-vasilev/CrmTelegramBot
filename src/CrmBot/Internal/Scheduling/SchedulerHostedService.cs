using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrmBot.Internal.Scheduling
{
    /// <summary>
    /// Service which runs scheduled tasks.
    /// </summary>
    public class SchedulerHostedService : HostedService
    {
        /// <summary>
        /// Event emitting unhandled exceptions.
        /// </summary>
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private readonly List<SchedulerTaskWrapper> scheduledTasks = new List<SchedulerTaskWrapper>();

        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks)
        {
            var referenceTime = DateTime.UtcNow;

            foreach (var scheduledTask in scheduledTasks)
            {
                this.scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTime = referenceTime
                });
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Execute tasks which should be executed at this time.
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        /// <summary>
        /// Executes all tasks which should be executed at this time.
        /// </summary>
        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var task in tasksThatShouldRun)
            {
                task.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await task.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (OperationCanceledException)
                        { }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }

        /// <summary>
        /// Wrapper for tasks.
        /// </summary>
        private class SchedulerTaskWrapper
        {
            public CrontabSchedule Schedule { get; set; }

            public IScheduledTask Task { get; set; }

            public DateTime LastRunTime { get; set; }

            public DateTime NextRunTime { get; set; }

            /// <summary>
            /// Updates time at which task should be run next time.
            /// </summary>
            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = Schedule.GetNextOccurrence(LastRunTime);
            }

            /// <summary>
            /// Check if the task should be run.
            /// </summary>
            /// <param name="currentTime">Relative time to compare.</param>
            /// <returns><c>true</c> if task should be executed.</returns>
            public bool ShouldRun(DateTime currentTime)
            {
                return NextRunTime < currentTime && LastRunTime != NextRunTime;
            }
        }
    }
}
