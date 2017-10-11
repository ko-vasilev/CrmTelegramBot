using CrmBot.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CrmBot.Services
{
    /// <summary>
    /// Provides access to data associated with the chats.
    /// </summary>
    public class ConversationService
    {
        public ConversationService(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        private readonly IMemoryCache cache;

        /// <summary>
        /// Retrieves a conversation data associated with the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <returns>Conversation information stored inbetween received messages.</returns>
        public ConversationContext GetAssociatedContext(long chatId)
        {
            return cache.GetOrCreate(GetCacheKey(chatId), cacheEntry =>
            {
                return new ConversationContext()
                {
                    ChatId = chatId
                };
            });
        }

        private static string GetCacheKey(long chatId) => nameof(ConversationService) + "-" + chatId;
    }
}
