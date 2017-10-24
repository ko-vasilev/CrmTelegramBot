using CrmBot.Bot.Commands.Models;
using CrmBot.Services;
using System;
using System.Threading.Tasks;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Contains helper methods for commands.
    /// </summary>
    public static class CommandUtils
    {
        /// <summary>
        /// Parse date from a command. If command text is empty, retrieves current date for associated user.
        /// </summary>
        /// <param name="commandContext">Command context.</param>
        /// <param name="crmService">Crm service.</param>
        /// <returns>Parsed date.</returns>
        public static async Task<DateTime> ParseDateFromMessage(CommandContext commandContext, CrmService crmService)
        {
            DateTime date;
            if (string.IsNullOrEmpty(commandContext.Message))
            {
                // Get "today" for current user
                var user = await crmService.GetUserAsync(commandContext.ChatId);
                date = DateTime.UtcNow.AddHours(user.TimeZone);
            }
            else if (!DateTime.TryParse(commandContext.Message, out date))
            {
                throw new FormatException("Could not parse the date, please use MM/dd or MM/dd/yyyy format.");
            }

            return date;
        }
    }
}
