using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CrmBot.DataAccess.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int TimeZone { get; set; }

        public List<TelegramChat> Chats { get; set; }
    }
}
