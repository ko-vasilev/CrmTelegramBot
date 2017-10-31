using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CrmBot.Services;
using System.Diagnostics;
using CrmBot.DataAccess;

namespace CrmBot.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ValuesController(AuthorizationService authService)
        {
            this.authService = authService;
        }

        private AuthorizationService authService;

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return new string[] { await authService.GetTokenAsync(123) };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
