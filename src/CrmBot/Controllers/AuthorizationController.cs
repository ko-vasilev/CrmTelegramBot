using CrmBot.Bot;
using CrmBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CrmBot.Controllers
{
    [Produces("application/json")]
    public class AuthorizationController : Controller
    {
        public AuthorizationController(TelegramBot bot, AuthorizationService authorizationService)
        {
            telegramBot = bot;
            this.authorizationService = authorizationService;
        }

        private readonly TelegramBot telegramBot;

        private readonly AuthorizationService authorizationService;

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

            if (await authorizationService.SetTokenAsync(chatId, accessToken))
            {
                await telegramBot.NotifySuccessfulConnectionAsync(chatId);
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}