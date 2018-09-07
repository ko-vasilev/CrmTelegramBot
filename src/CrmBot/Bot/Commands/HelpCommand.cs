using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.Internal;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Display a help message.
    /// </summary>
    public class HelpCommand : ICommand
    {
        public HelpCommand(AppSettings settings)
        {
            this.settings = settings;
        }

        private AppSettings settings;

        public CommandContext CommandContext { get; set; }

        public Task<ICommandExecutionResult> HandleCommand()
        {
            return Task.FromResult(new TextResult()
            {
                TextMessage =
$@"List of available commands:
/{CommandList.DailyReport} `date` - Start creating a daily report. After submitting the command, start entering a daily report text. Multiple messages are supported
/{CommandList.Jobs} `date` - Get jobs summary for a day
/{CommandList.JobsList} `date` - Get list of jobs for a day
/{CommandList.DailyReportNotificationsSubscribe} `hour`_(default: 21)_ - Subscribe for daily report notifications. After the `hour` if you have not submitted a daily report (but should have), you will receive a notification
/{CommandList.CheckDailyReport} `date` - Check whether or not you have submitted a daily report for specified date
/{CommandList.Connect} - Generate a link from which you can authorize the bot in CRM

A `date` parameter in commands is optional, if omitted, current date will be used instead.

If you have any questions, suggestions or bug reports, please contact [Konstantin Vasilev](tg://user?id={settings.HelpUserTelegramId})
",
                TextFormat = Telegram.Bot.Types.Enums.ParseMode.Markdown
            } as ICommandExecutionResult);
        }
    }
}
