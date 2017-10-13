using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Used to notify user that command they have entered does not exist.
    /// </summary>
    public class NotifyUnknownCommandCommand : ICommand
    {
        public CommandContext CommandContext { get; set; }

        public Task<ICommandExecutionResult> HandleCommand()
        {
            return Task.FromResult(new TextResult("Unknown command") as ICommandExecutionResult);
        }
    }
}
