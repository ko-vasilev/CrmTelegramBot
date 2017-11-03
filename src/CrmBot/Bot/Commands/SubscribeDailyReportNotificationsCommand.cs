using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess;
using CrmBot.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Subscribe user to receive notifications when they forget to submit a daily report.
    /// </summary>
    public class SubscribeDailyReportNotificationsCommand : ICommand
    {
        public SubscribeDailyReportNotificationsCommand(NotificationSubscriptionService subscriptionService, IAppUnitOfWorkFactory unitOfWork)
        {
            this.subscriptionService = subscriptionService;
            uow = unitOfWork;
        }

        private readonly NotificationSubscriptionService subscriptionService;

        private readonly IAppUnitOfWorkFactory uow;

        public CommandContext CommandContext { get; set; }

        public async Task<ICommandExecutionResult> HandleCommand()
        {
            short checkTime;
            if (!string.IsNullOrWhiteSpace(CommandContext.Message))
            {
                if (!short.TryParse(CommandContext.Message, out checkTime))
                {
                    throw new FormatException("Could not parse notification time");
                }
            }
            else
            {
                checkTime = 21;
            }

            int currentUserId;
            using (var database = uow.Create())
            {
                var chat = await database.TelegramChats.FirstAsync(ch => ch.ChatId == CommandContext.ChatId && ch.UserId.HasValue);
                currentUserId = chat.UserId.Value;
            }

            await subscriptionService.NotificationsSubscribe(currentUserId, checkTime, DataAccess.Models.NotificationType.MissDailyReport);
            return new TextResult($"Now you will receive notifications if you miss daily report after {checkTime} o'clock (your local time)");
        }
    }
}
