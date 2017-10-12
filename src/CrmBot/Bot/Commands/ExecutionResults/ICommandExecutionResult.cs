using System.Threading.Tasks;
using Telegram.Bot;

namespace CrmBot.Bot.Commands.ExecutionResults
{
    /// <summary>
    /// Execution result of a command.
    /// </summary>
    public interface ICommandExecutionResult
    {
        /// <summary>
        /// Sends the command execution result to user.
        /// </summary>
        /// <param name="bot">Telegram bot to be used for sending a message.</param>
        /// <param name="chatId">Id of the chat.</param>
        Task RenderResultAsync(TelegramBotClient bot, long chatId);
    }
}
