using System.ComponentModel.DataAnnotations;

namespace CrmBot.DataAccess.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int CrmUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TimeZoneCode { get; set; }

        public int BranchId { get; set; }

        public TelegramChat Chat { get; set; }
    }
}
