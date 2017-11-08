﻿using CrmBot.Bot;
using CrmBot.DataAccess.Models;
using CrmBot.DataAccess.Services;
using CrmBot.Internal.Scheduling;
using CrmBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BranchHoliday = SaritasaApi.Entities.BranchHoliday;

namespace CrmBot.PeriodicTasks
{
    /// <summary>
    /// Task to search through users and notify them if they have not submitted a daily report today yet.
    /// </summary>
    public class CheckSubmittedDailyReportsTask : IScheduledTask
    {
        public CheckSubmittedDailyReportsTask(
            CrmService crmService,
            TelegramBot telegramBot,
            NotificationSubscriptionService subscriptionService)
        {
            this.crmService = crmService;
            this.telegramBot = telegramBot;
            this.subscriptionService = subscriptionService;
        }

        private readonly CrmService crmService;

        private readonly TelegramBot telegramBot;

        private readonly NotificationSubscriptionService subscriptionService;

        public TimeSpan RepeatFrequency => TimeSpan.FromHours(1);

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var checkStart = DateTime.UtcNow;
            var pendingSubscriptions = await subscriptionService.GetPendingSubscriptions(checkStart, EventType.MissDailyReport);

            foreach (var subscription in pendingSubscriptions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var user = subscription.User;
                DateTime userCurrentDate = TimeZoneInfo.ConvertTimeFromUtc(checkStart, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneCode));
                // Do not notify user if notification time has not passed yet
                if (userCurrentDate.Hour < subscription.CheckTime)
                {
                    continue;
                }

                var shouldSubmitDailyReport = await ShouldSubmitDailyReport(user, userCurrentDate);
                cancellationToken.ThrowIfCancellationRequested();
                if (shouldSubmitDailyReport && !await crmService.DailyReportExists(user.Chat.ChatId, userCurrentDate))
                {
                    await telegramBot.NotifyMissedDailyReportAsync(user.Chat.ChatId);
                }
                subscription.LastCheck = checkStart;
                await subscriptionService.UpdateSubscription(subscription);
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
