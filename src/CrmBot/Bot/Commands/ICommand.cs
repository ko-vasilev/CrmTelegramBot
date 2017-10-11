using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using System;
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
        CommandContext CommandContext { set; }

        /// <summary>
        /// Executes current command.
        /// </summary>
        /// <returns>Result of command execution.</returns>
        Task<CommandExecutionResult> HandleCommand();
    }
}
