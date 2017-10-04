using CrmBot.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace CrmBot.Services
{
    /// <summary>
    /// Manages persistent storage of authentication codes.
    /// </summary>
    public class AuthenticationStoreService
    {
        /// <summary>
        /// .ctor
        /// </summary>
        public AuthenticationStoreService(CloudStorageAccount storageAccount, IDataProtectionProvider dataProtectionProvider)
        {
            this.storageAccount = storageAccount;
            dataProtector = dataProtectionProvider.CreateProtector("AccessToken");
        }

        private readonly CloudStorageAccount storageAccount;

        private readonly IDataProtector dataProtector;

        /// <summary>
        /// Get authentication key by chat id.
        /// </summary>
        /// <param name="chatId">Id of associated chat.</param>
        /// <returns>Authentication key info, or <c>null</c>.</returns>
        public async Task<AuthenticationData> GetKeyAsync(int chatId)
        {
            var table = await GetStorageTable();
            var retrieveResult = await table.ExecuteAsync(TableOperation.Retrieve<AuthenticationData>("", chatId.ToString()));

            var result = retrieveResult.Result as AuthenticationData;
            if (result?.AccessToken != null)
            {
                result.AccessToken = dataProtector.Unprotect(result.AccessToken);
            }
            return result;
        }

        /// <summary>
        /// Update access token associated with the chat.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="accessToken">Access token.</param>
        /// <returns><c>true</c> if update succeed.</returns>
        public async Task<bool> UpdateKeyAsync(int chatId, string accessToken)
        {
            var table = await GetStorageTable();
            var relatedEntity = await table.ExecuteAsync(TableOperation.Retrieve("", chatId.ToString()));
            if (relatedEntity.Result == null)
            {
                return false;
            }

            string protectedAccessToken = dataProtector.Protect(accessToken);
            await table.ExecuteAsync(TableOperation.Merge(new AuthenticationData(chatId, protectedAccessToken, relatedEntity.Etag)));
            return true;
        }

        /// <summary>
        /// Register information about chat being able to have an associated access token.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        public async Task RegisterChatAsync(int chatId)
        {
            var table = await GetStorageTable();
            await table.ExecuteAsync(TableOperation.InsertOrMerge(new AuthenticationData(chatId, null)));
        }

        /// <summary>
        /// Get a reference to storage table with the data.
        /// </summary>
        private async Task<CloudTable> GetStorageTable()
        {
            const string tableName = "AuthenticationKeys";
            var table = storageAccount.CreateCloudTableClient().GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
