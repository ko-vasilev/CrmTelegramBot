using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Command to notify user about successful authorization. As a side-effect, it will also reset used http client to make requests for the user/chat.
    /// </summary>
    public class NotifySuccessfulConnectionCommand : ICommand
    {
        public NotifySuccessfulConnectionCommand(IAppUnitOfWorkFactory uowFactory)
        {
            this.uowFactory = uowFactory;
        }

        private readonly IAppUnitOfWorkFactory uowFactory;

        /// <inheritdoc />
        public CommandContext CommandContext { get; set; }

        /// <inheritdoc />
        public async Task<ICommandExecutionResult> HandleCommand()
        {
            using (var database = uowFactory.Create())
            {
                var me = await database.Users.FirstOrDefaultAsync(u => u.Chat.ChatId == CommandContext.ChatId);
                return new TextResult($"You were identified as {me.FirstName} {me.LastName}. Now you can access some of the CRM functionality from here.");
            }
        }
    }
}
