namespace CrmBot.Bot.Commands.Models
{
    /// <summary>
    /// Command execution context.
    /// </summary>
    public class ExecutionContext
    {
        /// <summary>
        /// Id of the chat.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Received message (excluding the command name itself).
        /// </summary>
        public string Message { get; set; }
    }
}
