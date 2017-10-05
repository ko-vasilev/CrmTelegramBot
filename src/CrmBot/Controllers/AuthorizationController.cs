using CrmBot.Bot;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CrmBot.Controllers
{
    [Produces("application/json")]
    public class AuthorizationController : Controller
    {
        public AuthorizationController(TelegramBot bot)
        {
            telegramBot = bot;
        }

        private readonly TelegramBot telegramBot;

        [HttpPost("/authorize/{chatId}", Name = "Authorize_Token")]
        public async Task<ActionResult> Authorize(int chatId, IFormCollection form)
        {
            var accessToken = form
                .FirstOrDefault(param => param.Key == "access_token")
                .Value
                .FirstOrDefault();

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest();
            }

            var success = await telegramBot.SetChatAccessToken(chatId, accessToken);
            if (!success)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}