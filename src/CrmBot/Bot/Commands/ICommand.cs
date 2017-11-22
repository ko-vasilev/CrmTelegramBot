using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Base bot command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Associated execution context.
        /// </summary>
        CommandContext CommandContext { get; set; }

        /// <summary>
        /// Executes current command.
        /// </summary>
        /// <returns>Result of command execution.</returns>
        Task<ICommandExecutionResult> HandleCommand();
    }
}
