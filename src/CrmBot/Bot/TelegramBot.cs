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

        private readonly AuthorizationService authorizationService;

        public TelegramBot(string apiKey, AuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
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

        public async Task<bool> SetChatAccessToken(int chatId, string accessToken)
        {
            var success = await authorizationService.SetTokenAsync(chatId, accessToken);
            if (success)
            {
                await botClient.SendTextMessageAsync(chatId, "Now you can access some of the CRM functionality from here.");
            }

            return success;
        }
    }
}
