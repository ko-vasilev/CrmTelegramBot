using CrmBot.Internal.Scheduling;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace CrmBot.PeriodicTasks
{
    /// <summary>
    /// Task to search through users and notify them if they have not submitted a daily report today yet.
    /// </summary>
    public class CheckSubmittedDailyReportsTask : IScheduledTask
    {
        public TimeSpan RepeatFrequency => TimeSpan.FromHours(1);

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // @TODO: implement daily report checks.
            return;
        }
    }
}
