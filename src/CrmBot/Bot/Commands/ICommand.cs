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
        /// Authorization service.
        /// </summary>
        Lazy<AuthorizationService> AuthorizationService { set; }

        /// <summary>
        /// Associated execution context.
        /// </summary>
        ExecutionContext ExecutionContext { set; }

        /// <summary>
        /// Executes current command.
        /// </summary>
        /// <returns>Result of command execution.</returns>
        Task<CommandExecutionResult> HandleCommand();
    }
}
