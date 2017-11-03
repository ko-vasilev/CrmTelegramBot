using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmBot.DataAccess.Models
{
    public enum NotificationType
    {
        MissDailyReport
    }

    public class NotificationSubscription
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        public short CheckTime { get; set; }

        public NotificationType NotificationType { get; set; }

        public DateTime? LastCheck { get; set; }
    }
}
