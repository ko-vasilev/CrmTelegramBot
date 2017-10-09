using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using System;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Command to notify user about successful authorization. As a side-effect, it will also reset used http client to make requests for the user/chat.
    /// </summary>
    public class NotifySuccessfulConnectionCommand : ICommand
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
            // TODO: better to handle this as event source.
            CrmService.Value.ForgetClient(ExecutionContext.ChatId);
            var me = await CrmService.Value.GetMeAsync(ExecutionContext.ChatId);

            return new CommandExecutionResult
            {
                TextMessage = $"You were identified as {me.FirstName} {me.LastName}. Now you can access some of the CRM functionality from here."
            };
        }
    }
}
