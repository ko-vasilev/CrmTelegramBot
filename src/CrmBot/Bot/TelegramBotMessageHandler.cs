using CrmBot.Bot.Commands;
using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CrmBot.Bot
{
    /// <summary>
    /// Handles messages coming from users.
    /// </summary>
    public class TelegramBotMessageHandler
    {
        /// <summary>
        /// .ctor
        /// </summary>
        public TelegramBotMessageHandler(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Handle a chat message.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="messageText">Text message.</param>
        /// <returns>Result of the command execution.</returns>
        public async Task<CommandExecutionResult> HandleMessage(long chatId, string messageText)
        {
            var executionContext = new ExecutionContext
            {
                ChatId = chatId,
                Message = messageText
            };
            ICommand command = new GetAuthorizationUrlCommand()
            {
                ExecutionContext = executionContext,
                AuthorizationService = serviceProvider.GetService<Lazy<AuthorizationService>>()
            };

            return await command.HandleCommand();
        }
    }
}
