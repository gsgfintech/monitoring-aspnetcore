using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Stratedgeme.Strategies
{
    [Authorize]
    [Route("api/strategies")]
    public class StrategiesApiController : Controller
    {
        private readonly StrategiesControllerUtils utils;

        public StrategiesApiController(StrategiesControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpPost("{name}/{version}")]
        public async Task<List<KeyValuePair<string, string>>> Get(string name, string version)
        {
            var result = await utils.Get(name, version);

            return result.Model?.Config?.ToList() ?? new List<KeyValuePair<string, string>>();
        }
    }
}
