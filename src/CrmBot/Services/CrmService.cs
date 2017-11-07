using CrmBot.Internal;
using CrmBot.Models;
using SaritasaApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrmBot.Services
{
    /// <summary>
    /// Contains methods for interacting with CRM.
    /// </summary>
    public class CrmService
    {
        public CrmService(CrmClientService clientService, AppSettings appSettings)
        {
            this.clientService = clientService;
            this.appSettings = appSettings;
        }

        private readonly CrmClientService clientService;

        private readonly AppSettings appSettings;

        /// <summary>
        /// Get Url to be used to get the access token.
        /// </summary>
        /// <param name="chatId">Id of the chat to be authorized.</param>
        /// <param name="internalChatId">Internal id of the chat to be used for validation.</param>
        /// <returns>Url.</returns>
        public string GenerateCrmAuthorizationUrl(long chatId, Guid internalChatId)
        {
            var callbackUri = $"{appSettings.AuthorizationCallbackUrl}/{chatId}";
            var authorizationUrlBuilder = new UriBuilder(appSettings.CrmAuthorizationUrl);
            authorizationUrlBuilder.Query = $"clientId={Uri.EscapeDataString(appSettings.CrmApplicationId)}&state={internalChatId}&redirecturi={Uri.EscapeDataString(callbackUri)}";
            return authorizationUrlBuilder.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Resets any stored information for the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        public void ForgetClient(long chatId)
        {
            clientService.ForgetClient(chatId);
        }

        /// <summary>
        /// Gets information about user associated with the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <returns>Information about user.</returns>
        public async Task<UserModel> GetUserAsync(long chatId)
        {
            var client = await clientService.GetClient(chatId);
            return new UserModel()
            {
                FirstName = client.Me.FirstName,
                LastName = client.Me.LastName,
                TimeZoneCode = client.Me.TimeZoneCode,
                Id = client.Me.Id,
                BranchId = client.Me.BranchId
            };
        }

        /// <summary>
        /// Creates a daily report.
        /// </summary>
        /// <param name="chatId">Id of the chat for which user the daily report should be created.</param>
        /// <param name="text">Text of the daily report.</param>
        /// <param name="dailyReportDate">Date of the daily report.</param>
        /// <param name="notifyUserIds">Ids of users to be notified with e-mails.</param>
        /// <returns><c>true</c> if daily report was successfully created.</returns>
        public async Task<bool> CreateDailyReportAsync(long chatId, string text, DateTime dailyReportDate, IEnumerable<int> notifyUserIds)
        {
            var client = await clientService.GetClient(chatId);
            return await client.UpdateDailyReportAsync(new DailyReport
            {
                text = text,
                date = dailyReportDate,
                notifyUserIds = notifyUserIds
            });
        }

        /// <summary>
        /// Get supervisors of a user associated with the chat.
        /// </summary>
        /// <param name="chatId">Chat id.</param>
        /// <returns>List of supervisers.</returns>
        public async Task<IEnumerable<User>> GetSupervisersAsync(long chatId)
        {
            var client = await clientService.GetClient(chatId);
            return await client.GetMySupervisers();
        }

        /// <summary>
        /// Get list of user jobs for specified date.
        /// </summary>
        /// <param name="chatId">Id of the associated user chat.</param>
        /// <param name="jobsDate">Date to search jobs for.</param>
        /// <returns>List of matching jobs.</returns>
        public async Task<List<Job>> GetJobsAsync(long chatId, DateTime jobsDate)
        {
            var client = await clientService.GetClient(chatId);
            return await client.GetMyJobs(jobsDate);
        }

        public async Task<bool> DailyReportExists(long chatId, DateTime dailyReportDate)
        {
            var client = await clientService.GetClient(chatId);
            var dailyReport = await client.GetMyDailyReports(dailyReportDate.Date, dailyReportDate.Date, 1);
            return dailyReport.Count > 0;
        }

        public async Task<CalendarDay> GetDayInfo(long chatId, DateTime day)
        {
            var client = await clientService.GetClient(chatId);
            return (await client.GetMyCalendar(day.Date, day.Date)).FirstOrDefault();
        }

        public async Task<List<BranchHoliday>> GetBranchHolidays(long chatId, DateTime from, DateTime to)
        {
            var client = await clientService.GetClient(chatId);
            return await client.GetBranchHolidays(from.Date, to.Date);
        }
    }
}
