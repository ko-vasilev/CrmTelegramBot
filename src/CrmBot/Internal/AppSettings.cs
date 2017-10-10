namespace CrmBot.Internal
{
    public class AppSettings
    {
        /// <summary>
        /// Contains an API key for the telegram bot.
        /// </summary>
        public string TelegramBotKey { get; set; }

        /// <summary>
        /// CRM url used to authorize the telegram bot.
        /// </summary>
        public string CrmAuthorizationUrl { get; set; }

        /// <summary>
        /// URL of the CRM website.
        /// </summary>
        public string CrmUrl { get; set; }

        /// <summary>
        /// Id of the current telegram bot registered in the CRM.
        /// </summary>
        public string CrmApplicationId { get; set; }

        /// <summary>
        /// Url to be used for callback when authorization succeeds in CRM.
        /// </summary>
        public string AuthorizationCallbackUrl { get; set; }
    }
}
