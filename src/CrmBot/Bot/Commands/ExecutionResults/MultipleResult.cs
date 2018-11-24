using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CrmBot.Bot.Commands.ExecutionResults
{
    /// <summary>
    /// Aggregation result.
    /// </summary>
    public class MultipleResult : ICommandExecutionResult
    {
        public MultipleResult(IEnumerable<ICommandExecutionResult> results)
        {
            Results = results ?? throw new ArgumentNullException(nameof(results));
        }

        /// <summary>
        /// Text message.
        /// </summary>
        public IEnumerable<ICommandExecutionResult> Results { get; set; }

        /// <inheritdoc />
        public async Task RenderResultAsync(TelegramBotClient bot, long chatId)
        {
            foreach (var result in Results)
            {
                try
                {
                    await result.RenderResultAsync(bot, chatId);
                }
                catch (Exception) { }
            }
        }
    }
}
