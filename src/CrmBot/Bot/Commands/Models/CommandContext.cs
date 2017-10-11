namespace CrmBot.Bot.Commands.Models
{
    /// <summary>
    /// Command execution context.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// Id of the chat.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Received message (excluding the command name itself).
        /// </summary>
        public string Message { get; set; }

        public string Command { get; set; }

        public string RawMessage { get; set; }
    }
}
