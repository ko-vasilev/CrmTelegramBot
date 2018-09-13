using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess;
using CrmBot.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Unsubscribe from receiving missed daily report notifications.
    /// </summary>
    public class UnSubscribeDailyReportNotificationsCommand : ICommand
    {
        public UnSubscribeDailyReportNotificationsCommand(NotificationSubscriptionService subscriptionService, IAppUnitOfWorkFactory unitOfWork)
        {
            this.subscriptionService = subscriptionService;
            uow = unitOfWork;
        }

        private readonly NotificationSubscriptionService subscriptionService;

        private readonly IAppUnitOfWorkFactory uow;

        public CommandContext CommandContext { get; set; }

        public async Task<ICommandExecutionResult> HandleCommand()
        {
            int currentUserId;
            using (var database = uow.Create())
            {
                var chat = await database.TelegramChats.FirstAsync(ch => ch.ChatId == CommandContext.ChatId && ch.UserId.HasValue);
                currentUserId = chat.UserId.Value;
            }

            await subscriptionService.NotificationsUnsubscribe(currentUserId, DataAccess.Models.EventType.MissDailyReport);
            return new TextResult("Successfully unsubscribed from receiving missing daily report notifications");
        }
    }
}
