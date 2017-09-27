using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace CrmBot.Internal
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
    }
}
