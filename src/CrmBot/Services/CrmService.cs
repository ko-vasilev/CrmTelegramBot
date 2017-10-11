using CrmBot.Internal;
using CrmBot.Models;
using System;
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
        public async Task<UserModel> GetMeAsync(long chatId)
        {
            var client = await clientService.GetClient(chatId);
            return new UserModel()
            {
                FirstName = client.FirstName,
                LastName = client.LastName
            };
        }

        public async Task CreateDailyReportAsync(long chatId, string text)
        {
            var client = await clientService.GetClient(chatId);
            await client.UpdateDailyReportAsync(new SaritasaApi.Entities.DailyReport
            {
                text = text,
                date = DateTime.UtcNow
            });
        }
    }
}
