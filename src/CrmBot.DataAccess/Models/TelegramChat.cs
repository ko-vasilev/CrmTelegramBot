using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmBot.DataAccess.Models
{
    public class TelegramChat
    {
        [Key]
        public long ChatId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        public User User { get; set; }
    }
}
