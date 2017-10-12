using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Command for generating an authorization URL.
    /// </summary>
    public class GetAuthorizationUrlCommand : ICommand
    {
        public GetAuthorizationUrlCommand(
            CrmService crmService,
            AuthorizationService authorizationService)
        {
            this.crmService = crmService;
            this.authorizationService = authorizationService;
        }

        private AuthorizationService authorizationService;

        private CrmService crmService;

        /// <inheritdoc />
        public CommandContext CommandContext { get; set; }

        /// <inheritdoc />
        public async Task<ICommandExecutionResult> HandleCommand()
        {
            var authorizationUrl = crmService.GenerateCrmAuthorizationUrl(CommandContext.ChatId);
            await authorizationService.RegisterChatAsync(CommandContext.ChatId);
            return new TextResult
            {
                TextMessage = @"
Open this link to authorize the bot:
" + authorizationUrl
            };
        }
    }
}
