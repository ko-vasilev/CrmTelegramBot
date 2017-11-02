using CrmBot.Internal.Scheduling;
using System;
using System.Threading.Tasks;
using System.Threading;
using CrmBot.DataAccess;
using System.Linq;
using CrmBot.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using CrmBot.DataAccess.Models;
using CrmBot.Bot;
using BranchHoliday = SaritasaApi.Entities.BranchHoliday;

namespace CrmBot.PeriodicTasks
{
    /// <summary>
    /// Task to search through users and notify them if they have not submitted a daily report today yet.
    /// </summary>
    public class CheckSubmittedDailyReportsTask : IScheduledTask
    {
        public CheckSubmittedDailyReportsTask(IAppUnitOfWorkFactory unitOfWork, CrmService crmService, TelegramBot telegramBot)
        {
            uow = unitOfWork;
            this.crmService = crmService;
            this.telegramBot = telegramBot;
        }

        private readonly IAppUnitOfWorkFactory uow;

        private readonly CrmService crmService;

        private readonly TelegramBot telegramBot;

        public TimeSpan RepeatFrequency => TimeSpan.FromHours(1);

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            List<TelegramChat> authorizedChats;
            using (var database = uow.Create())
            {
                authorizedChats = await database.TelegramChats.Where(ch => ch.User != null && ch.AccessToken != null).ToListAsync();
                foreach (var chat in authorizedChats)
                {
                    await database.Entry(chat).Reference(ch => ch.User).LoadAsync();
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var chat in authorizedChats)
            {
                // TODO: convert to user's local time
                DateTime userCurrentDate = DateTime.Now;

                var shouldSubmitDailyReport = await ShouldSubmitDailyReport(chat.User, userCurrentDate);
                cancellationToken.ThrowIfCancellationRequested();
                if (shouldSubmitDailyReport && !await crmService.DailyReportExists(chat.ChatId, userCurrentDate))
                {
                    await telegramBot.NotifyMissedDailyReportAsync(chat.ChatId);
                }
            }
        }

        private async Task<bool> ShouldSubmitDailyReport(User user, DateTime userCurrentDate)
        {
            var chatId = user.Chat.ChatId;
            var branchHoliday = (await GetBranchesHolidays(chatId))
                .FirstOrDefault(br => br.branchId == user.BranchId && br.date.Date == userCurrentDate.Date);
            var userDayInfo = await crmService.GetDayInfo(chatId, userCurrentDate);

            var isOfficeHoliday = branchHoliday != null;
            if (isOfficeHoliday)
            {
                if (userDayInfo != null && userDayInfo.isWorkDay)
                {
                    return true;
                }
                return false;
            }

            if (userDayInfo != null && userDayInfo.isWorkDay == false)
            {
                return false;
            }

            return true;
        }

        private List<BranchHoliday> branchHolidays;

        private async Task<List<BranchHoliday>> GetBranchesHolidays(long chatId)
        {
            if (branchHolidays != null)
            {
                return branchHolidays;
            }
            var allTimeZones = TimeZoneInfo.GetSystemTimeZones();

            // get "current" days for all possible time zones
            var earliestTimeZoneDiff = allTimeZones.Min(tz => tz.BaseUtcOffset);
            var latestTimeZoneDiff = allTimeZones.Max(tz => tz.BaseUtcOffset);
            var earliestNowDay = DateTime.UtcNow.Add(earliestTimeZoneDiff).Date;
            var latestNowDay = DateTime.UtcNow.Add(latestTimeZoneDiff).Date;

            branchHolidays = await crmService.GetBranchHolidays(chatId, earliestNowDay, latestNowDay);
            return branchHolidays;
        }
    }
}
