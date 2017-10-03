using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace CrmBot.Services
{
    public class AuthorizationService
    {
        public AuthorizationService(IMemoryCache memoryCache, AuthenticationStoreService tokenStoreService)
        {
            cache = memoryCache;
            tokenStore = tokenStoreService;
        }

        private readonly IMemoryCache cache;

        private readonly AuthenticationStoreService tokenStore;

        /// <summary>
        /// Get access token associated with the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <returns>Access token, or <c>null</c> if no access token associated with the chat.</returns>
        public async Task<string> GetToken(int chatId)
        {
            var key = GetCacheKey(chatId);

            var token = await cache.GetOrCreateAsync(key, async entry =>
            {
                var keyEntry = await tokenStore.GetKeyAsync(chatId);
                return keyEntry?.AccessToken;
            });

            return token;
        }

        /// <summary>
        /// Set access token for a chat. Existing access token will be overridden.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="token">Access token.</param>
        public async Task SetToken(int chatId, string token)
        {
            cache.Set(GetCacheKey(chatId), token);
            await tokenStore.UpdateKeyAsync(chatId, token);
        }

        private static string GetCacheKey(int primaryKey) => "AuthorizationToken-" + primaryKey;
    }
}
