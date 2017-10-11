using System;

namespace CrmBot.Models
{
    /// <summary>
    /// Contains chat data to be stored in between message updates.
    /// </summary>
    public class ConversationContext
    {
        /// <summary>
        /// Id of the associated chat.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Allows to override which command should handle messages for current conversation.
        /// </summary>
        public Type CurrentExecutingCommand { get; set; }

        /// <summary>
        /// Any additional data which should be stored.
        /// </summary>
        public object ConversationData { get; set; }
    }
}
