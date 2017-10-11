using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    public class UpdateDailyReportCommand : ICommand
    {
        public UpdateDailyReportCommand(CrmService crmService)
        {
            this.crmService = crmService;
        }

        private CrmService crmService;

        /// <inheritdoc />
        public CommandContext CommandContext { get; set; }

        public async Task<CommandExecutionResult> HandleCommand()
        {
            await crmService.CreateDailyReportAsync(CommandContext.ChatId, CommandContext.Message);

            return new CommandExecutionResult()
            {
                TextMessage = "Success"
            };
        }
    }
}
