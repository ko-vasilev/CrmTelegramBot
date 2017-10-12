using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CrmBot.Bot.Commands.ExecutionResults
{
    /// <summary>
    /// Result represented as a text message.
    /// </summary>
    public class TextResult : ICommandExecutionResult
    {
        public TextResult()
        {
        }

        public TextResult(string message)
        {
            TextMessage = message;
        }

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

        /// <inheritdoc />
        public async Task RenderResultAsync(TelegramBotClient bot, long chatId)
        {
            await bot.SendTextMessageAsync(chatId, TextMessage, TextFormat, replyMarkup: AdditionalMarkup);
        }
    }
}
