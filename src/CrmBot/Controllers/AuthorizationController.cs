using CrmBot.Bot;
using CrmBot.DataAccess.Models;
using CrmBot.DataAccess.Services;
using CrmBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saritasa.Tools.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CrmBot.Controllers
{
    [Produces("application/json")]
    public class AuthorizationController : Controller
    {
        public AuthorizationController(
            TelegramBot bot,
            AuthorizationService authorizationService,
            CrmService crmService,
            UserService userService,
            TelegramChatService telegramChatService)
        {
            telegramBot = bot;
            this.authorizationService = authorizationService;
            this.crmService = crmService;
            this.userService = userService;
            this.telegramChatService = telegramChatService;
        }

        private readonly TelegramBot telegramBot;
        private readonly AuthorizationService authorizationService;
        private readonly CrmService crmService;
        private readonly UserService userService;
        private readonly TelegramChatService telegramChatService;

        [HttpPost("/authorize/{chatId}", Name = "Authorize_Token")]
        public async Task<ActionResult> Authorize(long chatId, IFormCollection form)
        {
            var accessToken = form
                .FirstOrDefault(param => param.Key == "access_token")
                .Value
                .FirstOrDefault();
            var internalChatId = form
                .FirstOrDefault(param => param.Key == "state")
                .Value
                .FirstOrDefault();

            Guid secureKey;
            if (string.IsNullOrEmpty(accessToken) || !Guid.TryParse(internalChatId, out secureKey))
            {
                return BadRequest();
            }

            try
            {
                await authorizationService.SetTokenAsync(chatId, secureKey, accessToken);
            }
            catch (DomainException)
            {
                return BadRequest();
            }
            var user = await crmService.GetUserAsync(chatId);
            var internalUserId = await userService.UpsertAsync(new User()
            {
                CrmUserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TimeZoneCode = user.TimeZoneCode,
                BranchId = user.BranchId
            });
            await telegramChatService.SetUserAsync(chatId, internalUserId);
            await telegramBot.NotifySuccessfulConnectionAsync(chatId);

            return Ok();
        }
    }
}
