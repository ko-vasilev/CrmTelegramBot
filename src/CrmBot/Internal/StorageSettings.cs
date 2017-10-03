namespace CrmBot.Internal
{
    /// <summary>
    /// Contains Azure storage settings.
    /// </summary>
    public class StorageSettings
    {
        /// <summary>
        /// Name of the Azure storage.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Access key of the storage.
        /// </summary>
        public string AccessKey { get; set; }
    }
}
