using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Command to notify user about successful authorization. As a side-effect, it will also reset used http client to make requests for the user/chat.
    /// </summary>
    public class NotifySuccessfulConnectionCommand : ICommand
    {
        public NotifySuccessfulConnectionCommand(CrmService crmService)
        {
            this.crmService = crmService;
        }

        private CrmService crmService;

        /// <inheritdoc />
        public CommandContext CommandContext { get; set; }

        /// <inheritdoc />
        public async Task<CommandExecutionResult> HandleCommand()
        {
            // TODO: better to handle this as event source.
            crmService.ForgetClient(CommandContext.ChatId);
            var me = await crmService.GetMeAsync(CommandContext.ChatId);

            return new CommandExecutionResult
            {
                TextMessage = $"You were identified as {me.FirstName} {me.LastName}. Now you can access some of the CRM functionality from here."
            };
        }
    }
}
