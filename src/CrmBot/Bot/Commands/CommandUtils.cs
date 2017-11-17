﻿using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess;
using CrmBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
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
        /// <param name="uowFactory">Factory for creating database connections.</param>
        /// <returns>Parsed date.</returns>
        public static async Task<DateTime> ParseDateFromMessage(CommandContext commandContext, IAppUnitOfWorkFactory uowFactory)
        {
            DateTime date;
            if (string.IsNullOrEmpty(commandContext.Message))
            {
                // Get "today" for current user
                User user;
                using (var database = uowFactory.Create())
                {
                    user = await database.Users.FirstOrDefaultAsync(u => u.Chat.ChatId == commandContext.ChatId);
                }
                date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneCode));
            }
            else if (!DateTime.TryParse(commandContext.Message, out date))
            {
                throw new FormatException("Could not parse the date, please use MM/dd or MM/dd/yyyy format.");
            }

            return date;
        }
    }
}
