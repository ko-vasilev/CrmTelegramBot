using CrmBot.Bot.Commands;
using CrmBot.Bot.Commands.ExecutionResults;
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
        public TelegramBotMessageHandler(IServiceProvider serviceProvider, ConversationService conversationService)
        {
            this.serviceProvider = serviceProvider;
            this.conversationService = conversationService;
        }

        private readonly IServiceProvider serviceProvider;

        private readonly ConversationService conversationService;

        /// <summary>
        /// Handle a chat message.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="messageText">Text message.</param>
        /// <returns>Result of the command execution.</returns>
        public async Task<ICommandExecutionResult> HandleMessage(long chatId, string messageText)
        {
            var command = GetAssociatedCommand(chatId, messageText, out var commandContext);
            command.CommandContext = commandContext;

            return await ExecuteCommand(command);
        }

        /// <summary>
        /// Handle message with a specific command.
        /// </summary>
        /// <typeparam name="T">Type of the command which should handle the message.</typeparam>
        /// <param name="chatId">Id of the related chat.</param>
        /// <param name="messageText">Text of the message.</param>
        /// <returns>Result of the command execution.</returns>
        public async Task<ICommandExecutionResult> HandleMessage<T>(long chatId, string messageText) where T: class, ICommand
        {
            var command = serviceProvider.GetService<T>();
            command.CommandContext = new CommandContext
            {
                ChatId = chatId,
                Command = "",
                Message = messageText,
                RawMessage = messageText
            };
            return await ExecuteCommand(command);
        }

        private async Task<ICommandExecutionResult> ExecuteCommand(ICommand command)
        {
            try
            {
                return await command.HandleCommand();
            }
            catch (Exception ex)
            {
                // TODO: log error.
                return new ErrorResult(ex);
            }
        }

        /// <summary>
        /// Regular expression used to extract the command name from a text message.
        /// </summary>
        private static readonly Regex CommandNameRegex = new Regex(@"^/[a-zA-Z]+(\s|$)", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Get a command which should handle the specified message.
        /// </summary>
        /// <param name="commandContext">Associated command context.</param>
        /// <returns>Instance of a command which should handle the message.</returns>
        private ICommand GetAssociatedCommand(long chatId, string messageText, out CommandContext commandContext)
        {
            commandContext = new CommandContext
            {
                ChatId = chatId,
                Message = messageText,
                RawMessage = messageText
            };
            var matchCommand = CommandNameRegex.Match(messageText);
            string commandName = string.Empty;
            if (matchCommand.Success)
            {
                commandName = matchCommand.Value
                    .TrimEnd()
                    .Substring(1) // Omit the / symbol
                    .ToLowerInvariant();
                commandContext.Message = messageText.Substring(matchCommand.Length).Trim();
            }
            commandContext.Command = commandName;

            var conversationContext = conversationService.GetAssociatedContext(chatId);
            if (conversationContext.CurrentExecutingCommand != null)
            {
                return serviceProvider.GetService(conversationContext.CurrentExecutingCommand) as ICommand;
            }

            if (commandName != string.Empty)
            {
                Type commandType = null;
                switch(commandName)
                {
                    case CommandList.Start:
                    case CommandList.Connect:
                        commandType = typeof(GetAuthorizationUrlCommand);
                        break;
                    case CommandList.DailyReport:
                        commandType = typeof(UpdateDailyReportCommand);
                        break;
                    case CommandList.Jobs:
                        commandType = typeof(GetDayJobProgressCommand);
                        break;
                    case CommandList.DailyReportNotificationsSubscribe:
                        commandType = typeof(SubscribeDailyReportNotificationsCommand);
                        break;
                }
                if (commandType != null)
                {
                    return serviceProvider.GetRequiredService(commandType) as ICommand;
                }
            }

            return serviceProvider.GetService<NotifyUnknownCommandCommand>();
        }
    }
}
