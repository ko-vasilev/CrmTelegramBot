using CrmBot.Services;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace CrmBot.Bot
{
    public class TelegramBot
    {
        private bool IsActivated { get; set; }

        private readonly TelegramBotClient botClient;

        public TelegramBot(string apiKey)
        {
            botClient = new TelegramBotClient(apiKey);
        }

        public void Activate()
        {
            if (IsActivated)
            {
                throw new Exception("Chat bot was already activated.");
            }

            botClient.OnMessage += BotClient_OnMessage;
            botClient.StartReceiving(new[] { UpdateType.MessageUpdate });
            IsActivated = true;
        }

        private async void BotClient_OnMessage(object sender, MessageEventArgs e)
        {
            var currentChatId = e.Message.Chat.Id;
            await botClient.SendChatActionAsync(currentChatId, ChatAction.Typing);

            await Task.Delay(TimeSpan.FromSeconds(2));

            await botClient.SendTextMessageAsync(currentChatId, "Hello");
        }

        /// <summary>
        /// Notify client that the bot has acquired access token and now can interact with CRM.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        public async Task NotifySuccessfulConnectionAsync(int chatId)
        {
            await botClient.SendChatActionAsync(chatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(chatId, "Now you can access some of the CRM functionality from here.");
        }
    }
}
