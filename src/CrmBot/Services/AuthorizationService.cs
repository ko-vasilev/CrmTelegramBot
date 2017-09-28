using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace CrmBot.Services
{
    public class AuthorizationService
    {
        public AuthorizationService(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        private readonly IMemoryCache cache;

        public Task<string> GetToken(int chatId)
        {
            var key = GetCacheKey(chatId);
            cache.Get<string>(key);

            if (!cache.TryGetValue(key, out string token))
            {
                return Task.FromResult<string>(null);
            }

            return Task.FromResult(token);
        }

        public Task SetToken(int chatId, string token)
        {
            cache.Set(GetCacheKey(chatId), token);

            return Task.FromResult(0);
        }

        private static string GetCacheKey(int primaryKey) => "AuthorizationToken-" + primaryKey;
    }
}
