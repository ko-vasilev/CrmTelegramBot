using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CrmBot.Bot.Commands
{
    public class UpdateDailyReportCommand : ICommand
    {
        public UpdateDailyReportCommand(CrmService crmService, ConversationService conversationService)
        {
            this.crmService = crmService;
            this.conversationService = conversationService;
        }

        /// <inheritdoc />
        public CommandContext CommandContext { get; set; }

        private CrmService crmService;

        private ConversationService conversationService;

        private const string CommandSubmit = "submit";

        private const string CommandCancel = "cancel";

        public async Task<CommandExecutionResult> HandleCommand()
        {
            var conversation = conversationService.GetAssociatedContext(CommandContext.ChatId);

            // This is the first message, should contain date of the daily report
            if (conversation.CurrentExecutingCommand == null)
            {
                if (DateTime.TryParse(CommandContext.Message, out var date))
                {
                    conversation.ConversationData = new DailyReportCommandData
                    {
                        DailyReportDate = date
                    };
                    conversation.CurrentExecutingCommand = typeof(UpdateDailyReportCommand);

                    return new CommandExecutionResult($"Creating a daily report for {date:D}. Please enter your daily report text.")
                    {
                        AdditionalMarkup = new ReplyKeyboardMarkup(new[] { new KeyboardButton(CommandSubmit), new KeyboardButton(CommandCancel) })
                        {
                            ResizeKeyboard = true
                        }
                    };
                }
                else
                {
                    return new CommandExecutionResult("Could not parse the date, please use MM/dd or MM/dd/yyyy format");
                }
            }
            var data = conversation.ConversationData as DailyReportCommandData;

            conversation.ConversationData = null;
            conversation.CurrentExecutingCommand = null;
            await crmService.CreateDailyReportAsync(CommandContext.ChatId, CommandContext.Message, data.DailyReportDate);

            return new CommandExecutionResult()
            {
                TextMessage = "Success",
                AdditionalMarkup = new ReplyKeyboardRemove()
            };
        }
    }
}
