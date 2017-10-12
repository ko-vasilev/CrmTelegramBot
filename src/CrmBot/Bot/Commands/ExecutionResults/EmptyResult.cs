using System.Threading.Tasks;
using Telegram.Bot;

namespace CrmBot.Bot.Commands.ExecutionResults
{
    /// <summary>
    /// An empty command result, will not send anything.
    /// </summary>
    public class EmptyResult : ICommandExecutionResult
    {
        /// <inheritdoc />
        public Task RenderResultAsync(TelegramBotClient bot, long chatId)
        {
            return Task.FromResult(0);
        }
    }
}
