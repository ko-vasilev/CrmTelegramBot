using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.Internal;
using System;
using System.Collections.Generic;
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

        private static readonly Dictionary<Type, string> CommandHelpText = new Dictionary<Type, string>
        {
            { typeof(GetAuthorizationUrlCommand), "Generate a link to authorize the bot in CRM." },
            {
                typeof(UpdateDailyReportCommand),
                $@"Create a daily report.
Format: /{CommandList.DailyReport} `date`

Simple usage example:
msg: `/{CommandList.DailyReport}`
msg: `Today was a good day!
I worked very hard and learned a lot of new cool stuff!`
msg: `submit`

Create a daily report from several messages:
msg: `/{CommandList.DailyReport}`
msg: `I helped mrs Oldrin today.`
msg: `And improved the CRM a lot meanwhile!`
msg: `submit`

Create a daily report for yesterday:
msg: `/{CommandList.DailyReport} -1`
msg: `Late DR because I worked overtime to deliver a feature.`
msg: `submit`"
            },
            {
                typeof(GetDayJobProgressCommand),
                $@"Get summary job stats for a day.
Simple usage example:
msg: `/{CommandList.Jobs}`

Get jobs summary for yesterday:
msg: `/{CommandList.Jobs} -1`"
            },
            {
                typeof(SubscribeDailyReportNotificationsCommand),
                $@"Subscribe for receiving daily report notifications.
Subscribe for receiving notifications if you have not submitted a daily report after 22:00 (your local time):
msg: `/{CommandList.DailyReportNotificationsSubscribe} 22`"
            },
            {
                typeof(GetDayJobsCommand),
                $@"Get detailed jobs list for a day.
Simple usage example:
msg: `/{CommandList.JobsList}`

Get jobs summary for yesterday:
msg: `/{CommandList.JobsList} -1`"
            },
            {
                typeof(CheckDailyReportExistsCommand),
                $@"Check whether you have submitted a daily report or not.
Simple usage example:
msg: `/{CommandList.CheckDailyReport}`

Check if you have submitted a daily report yesterday:
msg: `/{CommandList.CheckDailyReport} -1`"
            },
            { typeof(UnSubscribeDailyReportNotificationsCommand), "Unsubscribe from missing daily report notifications." },
            {
                typeof(HelpCommand),
                @"Helpception 🤔"
            }
        };

        public Task<ICommandExecutionResult> HandleCommand()
        {
            if (string.IsNullOrEmpty(CommandContext.Message))
            {
                return Task.FromResult(new TextResult()
                {
                    TextMessage =
    $@"List of available commands:
/{CommandList.DailyReport} `date` - Start creating a daily report. After submitting the command, start entering a daily report text. Multiple messages are supported
/{CommandList.Jobs} `date` - Get jobs summary for a day
/{CommandList.JobsList} `date` - Get list of jobs for a day
/{CommandList.DailyReportNotificationsSubscribe} `hour`_(default: 21)_ - Subscribe for daily report notifications. After the `hour` if you have not submitted a daily report (but should have), you will receive a notification
/{CommandList.DailyReportNotificationsUnsubscribe} - Unsubscribe from missing daily report notifications
/{CommandList.CheckDailyReport} `date` - Check whether or not you have submitted a daily report for specified date
/{CommandList.Connect} - Generate a link from which you can authorize the bot in CRM
/{CommandList.Help} `command` - Get information about available commands. If `command` is specified, a detailed help text regarding the command will be shown

A `date` parameter in commands is optional, if omitted, current date will be used instead.

If you have any questions, suggestions or bug reports, please contact [Konstantin Vasilev](tg://user?id={settings.HelpUserTelegramId})
",
                    TextFormat = Telegram.Bot.Types.Enums.ParseMode.Markdown
                } as ICommandExecutionResult);
            }

            Type associatedCommand;
            if (CommandList.PublicCommands.TryGetValue(CommandContext.Message, out associatedCommand))
            {
                string associatedCommandText;
                if (CommandHelpText.TryGetValue(associatedCommand, out associatedCommandText))
                {
                    return Task.FromResult(new TextResult(associatedCommandText)
                    {
                        TextFormat = Telegram.Bot.Types.Enums.ParseMode.Markdown
                    } as ICommandExecutionResult);
                }
            }

            return Task.FromResult(new TextResult("Unknown command") as ICommandExecutionResult);
        }
    }
}
