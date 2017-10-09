﻿using CrmBot.Bot.Commands;
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
            SetupCommand(command, executionContext);

            return await command.HandleCommand();
        }

        /// <summary>
        /// Handle message with a specific command.
        /// </summary>
        /// <typeparam name="T">Type of the command which should handle the message.</typeparam>
        /// <param name="chatId">Id of the related chat.</param>
        /// <param name="messageText">Text of the message.</param>
        /// <returns>Result of the command execution.</returns>
        public async Task<CommandExecutionResult> HandleMessage<T>(long chatId, string messageText) where T: class, ICommand, new()
        {
            var command = new T();
            SetupCommand(command, new ExecutionContext(chatId, messageText));
            return await command.HandleCommand();
        }

        private void SetupCommand(ICommand command, ExecutionContext executionContext)
        {
            command.ExecutionContext = executionContext;
            command.AuthorizationService = serviceProvider.GetService<Lazy<AuthorizationService>>();
            command.CrmService = serviceProvider.GetService<Lazy<CrmService>>();
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

            if (commandName != string.Empty)
            {
                switch(commandName)
                {
                    case CommandList.Start:
                    case CommandList.Connect:
                        return new GetAuthorizationUrlCommand();
                }
            }

            // TODO: implement handling "no suitable command" situations
            return new GetAuthorizationUrlCommand();
        }
    }
}