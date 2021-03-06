﻿using CrmBot.Bot.Commands;
using CrmBot.Bot.Commands.ExecutionResults;
using System;
using System.Net;
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

        private readonly TelegramBotMessageHandler commandHandler;

        public TelegramBot(string apiKey, TelegramBotMessageHandler commandHandler, IWebProxy proxy)
        {
            botClient = new TelegramBotClient(apiKey, proxy);
            this.commandHandler = commandHandler;
        }

        public void Activate()
        {
            if (IsActivated)
            {
                throw new Exception("Chat bot was already activated.");
            }

            botClient.OnMessage += BotClient_OnMessage;
            botClient.StartReceiving(new[] { UpdateType.Message });
            IsActivated = true;
        }

        private async void BotClient_OnMessage(object sender, MessageEventArgs e)
        {
            var currentChatId = e.Message.Chat.Id;

            var result = await commandHandler.HandleMessage(e.Message.From.Id, currentChatId, e.Message.Text);
            await result.RenderResultAsync(botClient, currentChatId);
        }

        /// <summary>
        /// Notify client that the bot has acquired access token and now can interact with CRM.
        /// </summary>
        /// <param name="chatId">Id of the chat.</param>
        public async Task NotifySuccessfulConnectionAsync(long chatId)
        {
            await botClient.SendChatActionAsync(chatId, ChatAction.Typing);
            var result = await commandHandler.HandleMessage<NotifySuccessfulConnectionCommand>(chatId, string.Empty);
            await result.RenderResultAsync(botClient, chatId);
        }

        public async Task NotifyMissedDailyReportAsync(long chatId)
        {
            await new TextResult("You have not submitted a daily report yet!").RenderResultAsync(botClient, chatId);
        }
    }
}
