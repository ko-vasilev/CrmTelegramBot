﻿using CrmBot.Internal;
using CrmBot.Models;
using SaritasaApi.Entities;
using System;
using System.Collections.Generic;
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
        /// <returns>Url.</returns>
        public string GenerateCrmAuthorizationUrl(long chatId)
        {
            var callbackUri = $"{appSettings.AuthorizationCallbackUrl}/{chatId}";
            var authorizationUrlBuilder = new UriBuilder(appSettings.CrmAuthorizationUrl);
            authorizationUrlBuilder.Query = $"clientId={Uri.EscapeDataString(appSettings.CrmApplicationId)}&redirecturi={Uri.EscapeDataString(callbackUri)}";
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
                TimeZone = client.Me.TimeZone
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
    }
}
