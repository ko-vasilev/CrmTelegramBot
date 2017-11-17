using CrmBot.Internal;
using CrmBot.Models;
using Microsoft.Extensions.Caching.Memory;
using SaritasaApi;
using System;
using System.Threading.Tasks;

namespace CrmBot.Services
{
    /// <summary>
    /// Creates CRM client connections.
    /// </summary>
    public class CrmClientService
    {
        public CrmClientService(
            IMemoryCache memoryCache,
            AuthorizationService authorizationService,
            AppSettings appSettings)
        {
            crmClientsCache = memoryCache;
            this.authorizationService = authorizationService;
            this.appSettings = appSettings;
        }

        private readonly IMemoryCache crmClientsCache;

        private readonly AuthorizationService authorizationService;

        private readonly AppSettings appSettings;

        /// <summary>
        /// Get a client connection associated with the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <returns>CRM client to be used to make requests in context of specified chat.</returns>
        private async Task<Client> GetClient(long chatId)
        {
            string cacheKey = GetCacheKey(chatId);
            return await crmClientsCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                var client = new Client(appSettings.CrmUrl);
                var accessKey = await authorizationService.GetTokenAsync(chatId);
                if (accessKey != null)
                {
                    client.SetToken(accessKey);
                    await client.GetMe();
                }
                return client;
            });
        }

        /// <summary>
        /// Performs a call to CRM API.
        /// </summary>
        /// <typeparam name="T">Type of object to be returned.</typeparam>
        /// <param name="chatId">Id of the related chat.</param>
        /// <param name="action">Logic to be performed against the api client.</param>
        public async Task<T> MakeApiCall<T>(long chatId, Func<Client, Task<T>> action)
        {
            try
            {
                var client = await GetClient(chatId);
                return await action(client);
            }
            catch (UnauthorizedException ex)
            {
                await authorizationService.ClearTokenAsync(chatId);
                crmClientsCache.Remove(GetCacheKey(chatId));

                throw new UnauthorizedException("Access was rejected for the account. To continue, authorize the bot again in CRM.", ex);
            }
        }

        /// <summary>
        /// Gets information about user associated with the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <returns>Information about user.</returns>
        public async Task<UserModel> GetUserAsync(long chatId)
        {
            var client = await GetClient(chatId);
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
        /// Clears information about specified chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        public void ForgetClient(long chatId)
        {
            crmClientsCache.Remove(GetCacheKey(chatId));
        }

        private static string GetCacheKey(long chatId) => nameof(CrmClientService) + "-client" + chatId;
    }
}
