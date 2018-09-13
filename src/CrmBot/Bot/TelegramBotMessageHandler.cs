using CrmBot.Bot.Commands;
using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.Internal;
using CrmBot.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
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
        public TelegramBotMessageHandler(
            IServiceProvider serviceProvider,
            ConversationService conversationService,
            AppSettings appSettings)
        {
            this.serviceProvider = serviceProvider;
            this.conversationService = conversationService;
            this.appSettings = appSettings;
        }

        private readonly IServiceProvider serviceProvider;

        private readonly ConversationService conversationService;

        private readonly AppSettings appSettings;

        /// <summary>
        /// Handle a chat message.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="messageText">Text message.</param>
        /// <returns>Result of the command execution.</returns>
        public async Task<ICommandExecutionResult> HandleMessage(int senderId, long chatId, string messageText)
        {
            try
            {
                var command = GetAssociatedCommand(senderId, chatId, messageText, out var commandContext);
                command.CommandContext = commandContext;

                return await ExecuteCommand(command);
            }
            catch (Exception ex)
            {
                var telemetry = serviceProvider.GetService<TelemetryClient>();
                telemetry.TrackException(ex);
                return new ErrorResult(ex);
            }
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
            var telemetry = serviceProvider.GetService<TelemetryClient>();
            using (var operation = telemetry.StartOperation<RequestTelemetry>(command.GetType().Name))
            {
                operation.Telemetry.Context.Properties.Add("ChatId", command.CommandContext.ChatId.ToString());

                try
                {
                    return await command.HandleCommand();
                }
                catch (Exception ex)
                {
                    telemetry.TrackException(ex);
                    return new ErrorResult(ex);
                }
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
        private ICommand GetAssociatedCommand(int senderId, long chatId, string messageText, out CommandContext commandContext)
        {
            messageText = messageText ?? string.Empty;
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
                if (CommandList.PublicCommands.ContainsKey(commandName))
                {
                    commandType = CommandList.PublicCommands[commandName];
                }

                if (commandType == null)
                {
                    var senderIsAdmin = senderId == appSettings.HelpUserTelegramId;
                    if (senderIsAdmin)
                    {
                        switch (commandName)
                        {
                            case AdminCommandList.Broadcast:
                                commandType = typeof(BroadcastMessageCommand);
                                break;
                        }
                    }
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
