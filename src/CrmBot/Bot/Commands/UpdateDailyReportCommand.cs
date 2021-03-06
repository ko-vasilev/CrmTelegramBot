﻿using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess;
using CrmBot.Services;
using Microsoft.ApplicationInsights;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CrmBot.Bot.Commands
{
    public class UpdateDailyReportCommand : ICommand
    {
        public UpdateDailyReportCommand(
            CrmService crmService,
            ConversationService conversationService,
            IAppUnitOfWorkFactory uowFactory,
            TelemetryClient telemetry)
        {
            this.crmService = crmService;
            this.conversationService = conversationService;
            this.uowFactory = uowFactory;
            this.telemetry = telemetry;
        }

        /// <inheritdoc />
        public CommandContext CommandContext { get; set; }

        private readonly CrmService crmService;
        private readonly ConversationService conversationService;
        private readonly IAppUnitOfWorkFactory uowFactory;
        private readonly TelemetryClient telemetry;

        private const string CommandSubmit = "submit";

        private const string CommandCancel = "cancel";

        public async Task<ICommandExecutionResult> HandleCommand()
        {
            var conversation = conversationService.GetAssociatedContext(CommandContext.ChatId);

            // This is the first message, should contain date of the daily report
            if (conversation.CurrentExecutingCommand == null)
            {
                var date = await CommandUtils.ParseDateFromMessage(CommandContext, uowFactory);
                conversation.ConversationData = new DailyReportCommandData
                {
                    DailyReportDate = date
                };
                conversation.CurrentExecutingCommand = typeof(UpdateDailyReportCommand);

                return new TextResult($"Creating a daily report for {date:D}. Please enter your daily report text.")
                {
                    AdditionalMarkup = new ReplyKeyboardMarkup(new[] { new KeyboardButton(CommandSubmit), new KeyboardButton(CommandCancel) })
                    {
                        ResizeKeyboard = true
                    }
                };
            }

            var data = conversation.ConversationData as DailyReportCommandData;

            if (CommandContext.RawMessage == CommandSubmit)
            {
                if (string.IsNullOrEmpty(data.Message))
                {
                    return new TextResult("Daily report with empty message cannot be created.");
                }

                conversation.ConversationData = null;
                conversation.CurrentExecutingCommand = null;

                var success = await UpdateDailyReportAsync(data);
                telemetry.TrackEvent("Daily report created");
                string resultMessage = success ? "Successfuly created daily report." : "Unexpected error occurred while updating the daily report.";
                return new TextResult(resultMessage)
                {
                    AdditionalMarkup = new ReplyKeyboardRemove()
                };
            }

            if (CommandContext.RawMessage == CommandCancel)
            {
                conversation.ConversationData = null;
                conversation.CurrentExecutingCommand = null;
                return new TextResult()
                {
                    TextMessage = "Daily report creation cancelled.",
                    AdditionalMarkup = new ReplyKeyboardRemove()
                };
            }

            if (!string.IsNullOrEmpty(data.Message))
            {
                data.Message += "\n";
            }
            data.Message += CommandContext.RawMessage;

            return new EmptyResult();
        }

        private async Task<bool> UpdateDailyReportAsync(DailyReportCommandData dailyReport)
        {
            var supervisers = await crmService.GetSupervisersAsync(CommandContext.ChatId);
            return await crmService.CreateDailyReportAsync(CommandContext.ChatId, dailyReport.Message, dailyReport.DailyReportDate, supervisers.Select(u => u.Id));
        }
    }
}
