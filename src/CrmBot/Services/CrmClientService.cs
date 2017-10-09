using Microsoft.Extensions.Caching.Memory;
using SaritasaApi;
using System.Threading.Tasks;

namespace CrmBot.Services
{
    /// <summary>
    /// Creates CRM client connections.
    /// </summary>
    public class CrmClientService
    {
        public CrmClientService(IMemoryCache memoryCache, AuthorizationService authorizationService)
        {
            crmClientsCache = memoryCache;
            this.authorizationService = authorizationService;
        }

        private readonly IMemoryCache crmClientsCache;

        private readonly AuthorizationService authorizationService;

        /// <summary>
        /// Get a client connection associated with the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <returns>CRM client to be used to make requests in context of specified chat.</returns>
        public async Task<Client> GetClient(long chatId)
        {
            string cacheKey = GetCacheKey(chatId);
            return await crmClientsCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                var client = new Client("http://localhost:5555/");
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
