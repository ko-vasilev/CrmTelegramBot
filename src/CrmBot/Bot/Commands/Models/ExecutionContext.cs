namespace CrmBot.Bot.Commands.Models
{
    /// <summary>
    /// Command execution context.
    /// </summary>
    public class ExecutionContext
    {
        public ExecutionContext(long chatId, string message)
        {
            ChatId = chatId;
            Message = message;
        }

        /// <summary>
        /// Id of the chat.
        /// </summary>
        public long ChatId { get; private set; }

        /// <summary>
        /// Received message (excluding the command name itself).
        /// </summary>
        public string Message { get; set; }
    }
}
