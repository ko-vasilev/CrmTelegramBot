﻿using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using System;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Command for generating an authorization URL.
    /// </summary>
    public class GetAuthorizationUrlCommand : ICommand
    {
        /// <inheritdoc />
        public Lazy<AuthorizationService> AuthorizationService { get; set; }

        /// <inheritdoc />
        public Lazy<CrmService> CrmService { get; set; }

        /// <inheritdoc />
        public ExecutionContext ExecutionContext { get; set; }

        /// <inheritdoc />
        public async Task<CommandExecutionResult> HandleCommand()
        {
            var authorizationUrl = CrmService.Value.GenerateCrmAuthorizationUrl(ExecutionContext.ChatId);
            await AuthorizationService.Value.RegisterChatAsync(ExecutionContext.ChatId);
            return new CommandExecutionResult
            {
                TextMessage = @"
Open this link to authorize the bot:
" + authorizationUrl
            };
        }
    }
}
