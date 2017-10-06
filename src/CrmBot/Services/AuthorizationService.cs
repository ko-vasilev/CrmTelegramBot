using CrmBot.Internal;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace CrmBot.Services
{
    public class AuthorizationService
    {
        public AuthorizationService(
            IMemoryCache memoryCache,
            AuthenticationStoreService tokenStoreService,
            AppSettings appSettings)
        {
            cache = memoryCache;
            tokenStore = tokenStoreService;
            this.appSettings = appSettings;
        }

        private readonly IMemoryCache cache;

        private readonly AuthenticationStoreService tokenStore;

        private readonly AppSettings appSettings;

        /// <summary>
        /// Get access token associated with the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <returns>Access token, or <c>null</c> if no access token associated with the chat.</returns>
        public async Task<string> GetTokenAsync(long chatId)
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
        /// <returns><c>true</c> if access token was successfully set.</returns>
        public async Task<bool> SetTokenAsync(long chatId, string token)
        {
            var success = await tokenStore.UpdateKeyAsync(chatId, token);
            if (success)
            {
                cache.Set(GetCacheKey(chatId), token);
            }
            return success;
        }

        /// <summary>
        /// Register information about chat being able to have an associated access token.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        public async Task RegisterChatAsync(long chatId)
        {
            await tokenStore.RegisterChatAsync(chatId);
        }

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

        private static string GetCacheKey(long primaryKey) => "AuthorizationToken-" + primaryKey;
    }
}
