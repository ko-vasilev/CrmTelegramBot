using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CrmBot.Bot.Commands.ExecutionResults
{
    /// <summary>
    /// Result represented as a text message sending to a specific chat.
    /// </summary>
    public class DirectTextResult : ICommandExecutionResult
    {
        public DirectTextResult(string message, long chatId)
        {
            TextMessage = message;
            ChatId = chatId;
        }

        /// <summary>
        /// Text message.
        /// </summary>
        public string TextMessage { get; set; } = "";

        /// <summary>
        /// Id of the chat where the message will be sent.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Text message format.
        /// </summary>
        public ParseMode TextFormat { get; set; } = ParseMode.Default;

        /// <inheritdoc />
        public async Task RenderResultAsync(TelegramBotClient bot, long chatId)
        {
            await bot.SendChatActionAsync(ChatId, ChatAction.Typing);
            await bot.SendTextMessageAsync(ChatId, TextMessage, TextFormat);
        }
    }
}
