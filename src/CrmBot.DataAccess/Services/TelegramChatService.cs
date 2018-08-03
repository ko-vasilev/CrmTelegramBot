using CrmBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Saritasa.Tools.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrmBot.DataAccess.Services
{
    public class TelegramChatService
    {
        public TelegramChatService(IAppUnitOfWorkFactory unitOfWork)
        {
            uow = unitOfWork;
        }

        private readonly IAppUnitOfWorkFactory uow;

        /// <summary>
        /// Set access token for a chat. Existing access token will be overridden.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="chatKey">A key of the chat used to ensure that chat operation is allowed.</param>
        /// <param name="token">Access token.</param>
        /// <returns><c>true</c> if access token was successfully set.</returns>
        public async Task SetTokenAsync(long chatId, Guid chatKey, string token)
        {
            using (var database = uow.Create())
            {
                var chat = await database.TelegramChats.FirstOrDefaultAsync(ch => ch.ChatId == chatId);
                if (chat == null)
                {
                    throw new NotFoundException("Cannot find chat with id " + chatId);
                }
                if (chat.SecureKey != chatKey)
                {
                    throw new ValidationException("Specified chat key does not match the configured key");
                }

                chat.AccessToken = token;
                await database.SaveChangesAsync();
            }
        }
        
        /// <summary>
         /// Get authentication key by chat id.
         /// </summary>
         /// <param name="chatId">Id of associated chat.</param>
         /// <returns>Authentication key info, or <c>null</c>.</returns>
        public async Task<string> GetTokenAsync(long chatId)
        {
            using (var database = uow.Create())
            {
                var chat = await database.TelegramChats.FirstOrDefaultAsync(ch => ch.ChatId == chatId);
                if (chat == null)
                {
                    throw new NotFoundException("Cannot find chat with id " + chatId);
                }

                return chat.AccessToken;
            }
        }

        /// <summary>
        /// Register information about chat being able to have an associated access token.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        public async Task<Guid> RegisterChatAsync(long chatId)
        {
            TelegramChat chat;
            using (var database = uow.Create())
            {
                chat = await database.TelegramChats.FindAsync(chatId);
                if (chat == null)
                {
                    var createdEntity = await database.TelegramChats.AddAsync(new TelegramChat()
                    {
                        ChatId = chatId
                    });
                    await database.SaveChangesAsync();
                    chat = createdEntity.Entity;
                }
            }
            return chat.SecureKey;
        }

        /// <summary>
        /// Associates a user with chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="userId">Id of the user.</param>
        public async Task SetUserAsync(long chatId, int userId)
        {
            using (var database = uow.Create())
            {
                var chat = await database.TelegramChats.FirstOrDefaultAsync(ch => ch.ChatId == chatId);
                if (chat == null)
                {
                    throw new NotFoundException("Cannot find chat with id " + chatId);
                }
                chat.UserId = userId;

                await database.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Clears access token associated with a chat.
        /// </summary>
        /// <param name="chatId">Id of chat which token should be cleared.</param>
        public async Task ClearTokenAsync(long chatId)
        {
            using (var database = uow.Create())
            {
                var chat = await database.TelegramChats.FirstOrDefaultAsync(ch => ch.ChatId == chatId);
                if (chat == null)
                {
                    throw new NotFoundException("Cannot find chat with id " + chatId);
                }
                if (chat.AccessToken != null)
                {
                    chat.AccessToken = null;
                    await database.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Get list of active users' chats.
        /// </summary>
        public async Task<IEnumerable<long>> GetActiveChats()
        {
            using (var database = uow.Create())
            {
                return await (
                    from chat in database.TelegramChats
                    where
                        chat.User != null
                        && chat.AccessToken != null
                    select chat.ChatId
                )
                .ToListAsync();
            }
        }
    }
}
