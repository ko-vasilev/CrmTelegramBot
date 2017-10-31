namespace CrmBot.Models
{
    /// <summary>
    /// Contains information about user.
    /// </summary>
    public class UserModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int TimeZone { get; set; }
    }
}
