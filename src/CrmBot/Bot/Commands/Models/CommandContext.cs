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

        /// <summary>
        /// Parsed command name.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Original message received from the user.
        /// </summary>
        public string RawMessage { get; set; }
    }
}
