using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess;
using CrmBot.Services;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Check whether or not user has submitted a daily report for a day.
    /// </summary>
    public class CheckDailyReportExistsCommand : ICommand
    {
        public CheckDailyReportExistsCommand(CrmService crmService, IAppUnitOfWorkFactory uowFactory)
        {
            this.crmService = crmService;
            this.uowFactory = uowFactory;
        }

        public CommandContext CommandContext { get; set; }

        private readonly CrmService crmService;
        private readonly IAppUnitOfWorkFactory uowFactory;

        public async Task<ICommandExecutionResult> HandleCommand()
        {
            var checkDate = await CommandUtils.ParseDateFromMessage(CommandContext, uowFactory);
            var dailyReportExists = await crmService.DailyReportExists(CommandContext.ChatId, checkDate);

            var dailyReportStatusText = dailyReportExists ?
                "have"
                : "*HAVE NOT*";
            return new TextResult($@"You {dailyReportStatusText} submitted a daily report for {checkDate:D}")
            {
                TextFormat = ParseMode.Markdown
            };
        }
    }
}
