using Microsoft.WindowsAzure.Storage.Table;

namespace CrmBot.Models
{
    /// <summary>
    /// Contains information for authentication.
    /// </summary>
    public class AuthenticationData : TableEntity
    {
        public AuthenticationData()
        {
            PartitionKey = string.Empty;
        }

        public AuthenticationData(int chatId, string accessToken)
            : this()
        {
            ChatId = chatId;
            AccessToken = accessToken;
        }

        private int chatId;
        /// <summary>
        /// Id of chat for which the access token was granted.
        /// </summary>
        public int ChatId
        {
            get
            {
                return chatId;
            }
            set
            {
                chatId = value;
                RowKey = value.ToString();
            }
        }

        /// <summary>
        /// Access token to be used for authentication.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
