using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmBot.DataAccess.Models
{
    public class TelegramChat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long ChatId { get; set; }

        /// <summary>
        /// Internal key associated with the chat.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SecureKey { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        public User User { get; set; }

        public string AccessToken { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
