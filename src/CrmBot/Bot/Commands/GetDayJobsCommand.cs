using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess;
using CrmBot.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Get list of all jobs for a day.
    /// </summary>
    public class GetDayJobsCommand : ICommand
    {
        public GetDayJobsCommand(CrmService crmService, IAppUnitOfWorkFactory uowFactory)
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

            var jobsList = new StringBuilder();
            foreach (var job in jobs.OrderBy(j => j.jobID))
            {
                jobsList.AppendLine();
                jobsList.AppendFormat(@"*{0}* ```
{1}```", ToTime(job.duration), job.text);
            }

            return new TextResult($@"Jobs for {jobDate:D}" + jobsList.ToString())
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
