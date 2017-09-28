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
        public AuthorizationController(AuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

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

            await authorizationService.SetToken(chatId, accessToken);

            return Ok();
        }
    }
}