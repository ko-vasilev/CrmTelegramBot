using CrmBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace CrmBot.DataAccess.Services
{
    public class NotificationSubscriptionService
    {
        public NotificationSubscriptionService(IAppUnitOfWorkFactory unitOfWork)
        {
            uow = unitOfWork;
        }

        private readonly IAppUnitOfWorkFactory uow;

        /// <summary>
        /// Subscribe user to specified notifications.
        /// </summary>
        /// <param name="userId">Id of the user who is subscribing.</param>
        /// <param name="checkTime">Time (from 0 to 23) after which the check should be performed.</param>
        /// <param name="notificationType">Type of notifications to subscribe to.</param>
        public async Task NotificationsSubscribe(int userId, short checkTime, EventType notificationType)
        {
            if (checkTime < 0 || checkTime > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(checkTime), "Notification check time must be in range from 0 to 23");
            }
            using (var database = uow.Create())
            {
                var subscription = await database.NotificationSubscriptions.FirstOrDefaultAsync(s => s.UserId == userId && s.EventType == notificationType);
                if (subscription == null)
                {
                    var subscriptionEntity = await database.NotificationSubscriptions.AddAsync(new NotificationSubscription()
                    {
                        EventType = notificationType,
                        UserId = userId
                    });
                    subscription = subscriptionEntity.Entity;
                }
                subscription.CheckTime = checkTime;
                await database.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets list of subscriptions.
        /// </summary>
        /// <param name="relativeDate">Only subscriptions with last trigger date later than this value will be returned.</param>
        /// <param name="eventType">Associated subscription's event type.</param>
        /// <returns>List of matching subscriptions.</returns>
        public async Task<List<NotificationSubscription>> GetPendingSubscriptions(DateTime relativeDate, EventType eventType)
        {
            using (var database = uow.Create())
            {
                var subscriptions = await (
                    from subscription in database.NotificationSubscriptions
                    join chat in database.TelegramChats on subscription.UserId equals chat.UserId
                    where
                        chat.User != null
                        && chat.AccessToken != null
                        && subscription.EventType == eventType
                        && (subscription.LastCheck == null || subscription.LastCheck.Value.Date < relativeDate.Date)
                    select subscription
                )
                .Include(subscription => subscription.User)
                .ThenInclude(user => user.Chat)
                .ToListAsync();

                return subscriptions;
            }
        }

        /// <summary>
        /// Save the existing subscription info.
        /// </summary>
        public async Task UpdateSubscription(NotificationSubscription subscription)
        {
            using (var database = uow.Create())
            {
                database.NotificationSubscriptions.Update(subscription);
                await database.SaveChangesAsync();
            }
        }
    }
}
