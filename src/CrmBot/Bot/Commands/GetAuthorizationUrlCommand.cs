using CrmBot.Bot.Commands.Models;
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
        public ExecutionContext ExecutionContext { get; set; }

        /// <inheritdoc />
        public async Task<CommandExecutionResult> HandleCommand()
        {
            return new CommandExecutionResult
            {
                TextMessage = "Hello"
            };
        }
    }
}
