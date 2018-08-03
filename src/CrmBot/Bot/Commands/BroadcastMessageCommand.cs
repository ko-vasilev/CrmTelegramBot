using CrmBot.Bot.Commands.ExecutionResults;
using CrmBot.Bot.Commands.Models;
using CrmBot.DataAccess.Services;
using CrmBot.Services;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Send a message to all active users.
    /// </summary>
    public class BroadcastMessageCommand : ICommand
    {
        public BroadcastMessageCommand(TelegramChatService chatService, ConversationService conversationService)
        {
            this.chatService = chatService;
            this.conversationService = conversationService;
        }

        public CommandContext CommandContext { get; set; }

        private readonly TelegramChatService chatService;
        private readonly ConversationService conversationService;

        private const string CommandCancel = "cancel";

        public async Task<ICommandExecutionResult> HandleCommand()
        {
            var conversation = conversationService.GetAssociatedContext(CommandContext.ChatId);

            if (conversation.CurrentExecutingCommand == null)
            {
                conversation.CurrentExecutingCommand = typeof(BroadcastMessageCommand);
                return new TextResult("The next message will be broadcasted to all users.")
                {
                    AdditionalMarkup = new ReplyKeyboardMarkup(new[] { new KeyboardButton(CommandCancel) })
                    {
                        ResizeKeyboard = true
                    }
                };
            }

            conversation.CurrentExecutingCommand = null;
            if (CommandContext.RawMessage == CommandCancel)
            {
                conversation.ConversationData = null;
                conversation.CurrentExecutingCommand = null;
                return new TextResult()
                {
                    TextMessage = "Action cancelled.",
                    AdditionalMarkup = new ReplyKeyboardRemove()
                };
            }

            var resultMessages = 
                (await chatService.GetActiveChats())
                .Select(chatId => (ICommandExecutionResult)new DirectTextResult(CommandContext.Message, chatId))
                .ToList();

            resultMessages.Add(new TextResult("Success")
            {
                AdditionalMarkup = new ReplyKeyboardRemove()
            });
            return new MultipleResult(resultMessages);
        }
    }
}
