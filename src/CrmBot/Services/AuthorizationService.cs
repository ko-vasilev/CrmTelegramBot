using CrmBot.DataAccess.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace CrmBot.Services
{
    public class AuthorizationService
    {
        public AuthorizationService(
            IMemoryCache memoryCache,
            TelegramChatService chatService,
            IDataProtectionProvider dataProtectionProvider)
        {
            cache = memoryCache;
            this.chatService = chatService;
            dataProtector = dataProtectionProvider.CreateProtector("AccessToken");
        }

        private readonly IMemoryCache cache;

        private readonly TelegramChatService chatService;

        private readonly IDataProtector dataProtector;

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
                var rawKey = await chatService.GetTokenAsync(chatId);
                return dataProtector.Unprotect(rawKey);
            });

            return token;
        }

        /// <summary>
        /// Set access token for a chat. Existing access token will be overridden.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="token">Access token.</param>
        /// <returns><c>true</c> if access token was successfully set.</returns>
        public async Task SetTokenAsync(long chatId, Guid chatKey, string token)
        {
            var protectedToken = dataProtector.Protect(token);
            await chatService.SetTokenAsync(chatId, chatKey, protectedToken);
            cache.Set(GetCacheKey(chatId), token);
        }

        /// <summary>
        /// Register information about chat being able to have an associated access token.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        public async Task<Guid> RegisterChatAsync(long chatId)
        {
            return await chatService.RegisterChatAsync(chatId);
        }

        private static string GetCacheKey(long primaryKey) => "AuthorizationToken-" + primaryKey;
    }
}
