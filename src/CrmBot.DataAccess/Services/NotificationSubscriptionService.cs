using CrmBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

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
        public async Task NotificationsSubscribe(int userId, short checkTime, NotificationType notificationType)
        {
            if (checkTime < 0 || checkTime > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(checkTime), "Notification check time must be in range from 0 to 23");
            }
            using (var database = uow.Create())
            {
                var subscription = await database.NotificationSubscriptions.FirstOrDefaultAsync(s => s.UserId == userId && s.NotificationType == notificationType);
                if (subscription == null)
                {
                    var subscriptionEntity = await database.NotificationSubscriptions.AddAsync(new NotificationSubscription()
                    {
                        NotificationType = notificationType,
                        UserId = userId
                    });
                    subscription = subscriptionEntity.Entity;
                }
                subscription.CheckTime = checkTime;
                await database.SaveChangesAsync();
            }
        }

    }
}
