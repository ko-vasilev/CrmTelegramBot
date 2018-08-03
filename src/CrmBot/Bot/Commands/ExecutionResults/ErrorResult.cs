using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CrmBot.Bot.Commands.ExecutionResults
{
    /// <summary>
    /// When command results in an error.
    /// </summary>
    public class ErrorResult : ICommandExecutionResult
    {
        public ErrorResult(Exception exception)
        {
            this.exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        private readonly Exception exception;

        public async Task RenderResultAsync(TelegramBotClient bot, long chatId)
        {
            await bot.SendChatActionAsync(chatId, ChatAction.Typing);
            await bot.SendTextMessageAsync(chatId, "Oops, something went wrong.");
        }
    }
}
