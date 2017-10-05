using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CrmBot.Bot.Commands.Models
{
    /// <summary>
    /// Result of a command execution.
    /// </summary>
    public class CommandExecutionResult
    {
        /// <summary>
        /// Text message.
        /// </summary>
        public string TextMessage { get; set; } = "";

        /// <summary>
        /// Text message format.
        /// </summary>
        public ParseMode TextFormat { get; set; } = ParseMode.Default;

        /// <summary>
        /// Any additional markup which should be provided with the message.
        /// </summary>
        public IReplyMarkup AdditionalMarkup { get; set; }
    }
}
