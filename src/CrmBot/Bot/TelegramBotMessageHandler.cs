using CrmBot.Bot.Commands;
using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.RegularExpressions;
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
            var executionContext = new ExecutionContext(chatId, messageText);
            var command = GetAssociatedCommand(executionContext);
            command.ExecutionContext = executionContext;
            command.AuthorizationService = serviceProvider.GetService<Lazy<AuthorizationService>>();

            return await command.HandleCommand();
        }

        /// <summary>
        /// Regular expression used to extract the command name from a text message.
        /// </summary>
        private static readonly Regex CommandNameRegex = new Regex(@"^/[a-zA-Z]+(\s|$)", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Get a command which should handle the specified message.
        /// </summary>
        /// <param name="executionContext">Associated execution context.</param>
        /// <returns>Instance of a command which should handle the message.</returns>
        private ICommand GetAssociatedCommand(ExecutionContext executionContext)
        {
            var matchCommand = CommandNameRegex.Match(executionContext.Message);
            string commandName = string.Empty;
            if (matchCommand.Success)
            {
                commandName = matchCommand.Value
                    .TrimEnd()
                    .Substring(1) // Omit the / symbol
                    .ToLowerInvariant();
                executionContext.Message = executionContext.Message.Substring(matchCommand.Length);
            }

            return new GetAuthorizationUrlCommand();
        }
    }
}
