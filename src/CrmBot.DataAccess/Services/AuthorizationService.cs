using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmBot.DataAccess.Services
{
    public class AuthorizationService
    {
        public AuthorizationService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        private readonly DatabaseContext databaseContext;

        /// <summary>
        /// Set access token for a chat. Existing access token will be overridden.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="token">Access token.</param>
        /// <returns><c>true</c> if access token was successfully set.</returns>
        public async Task<bool> SetTokenAsync(long chatId, string token)
        {
            var userKey = databaseContext.UserKeys.FirstOrDefault(k => k.User.Chats.Any(ch => ch.ChatId == chatId));
            if (userKey == null)
            {
                // check if chat is registered for associating 
            }
            var relatedEntity = await table.ExecuteAsync(TableOperation.Retrieve("", chatId.ToString()));
            if (relatedEntity.Result == null)
            {
                return false;
            }

            string protectedAccessToken = dataProtector.Protect(accessToken);
            await table.ExecuteAsync(TableOperation.Merge(new AuthenticationData(chatId, protectedAccessToken, relatedEntity.Etag)));
            var success = await tokenStore.UpdateKeyAsync(chatId, token);
            if (success)
            {
                cache.Set(GetCacheKey(chatId), token);
            }
            return success;
        }
    }
}
