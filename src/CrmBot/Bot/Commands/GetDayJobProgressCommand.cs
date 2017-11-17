using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess;
using CrmBot.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Get summary of jobs for a day.
    /// </summary>
    public class GetDayJobProgressCommand : ICommand
    {
        public GetDayJobProgressCommand(CrmService crmService, IAppUnitOfWorkFactory uowFactory)
        {
            this.crmService = crmService;
            this.uowFactory = uowFactory;
        }

        public CommandContext CommandContext { get; set; }

        private readonly CrmService crmService;
        private readonly IAppUnitOfWorkFactory uowFactory;

        public async Task<ICommandExecutionResult> HandleCommand()
        {
            var jobDate = await CommandUtils.ParseDateFromMessage(CommandContext, uowFactory);
            var jobs = await crmService.GetJobsAsync(CommandContext.ChatId, jobDate);

            var billable =
                jobs.Where(j => j.billable)
                .Select(j => j.duration)
                .DefaultIfEmpty(0)
                .Sum();
            var unbillable =
                jobs.Where(j => !j.billable)
                .Select(j => j.duration)
                .DefaultIfEmpty(0)
                .Sum();
            var total = billable + unbillable;

            return new TextResult($@"Jobs for {jobDate:D}
Total: *{ToTime(total)}*
Billable: *{ToTime(billable)}*
Non-Billable: *{ToTime(unbillable)}*")
            {
                TextFormat = ParseMode.Markdown
            };
        }

        /// <summary>
        /// Format minutes.
        /// </summary>
        private string ToTime(int minutes)
        {
            return TimeSpan.FromMinutes(minutes).ToString(@"hh\:mm");
        }
    }
}
