using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Stratedgeme.Strategies
{
    //[Authorize]
    [Route("api/strategies")]
    public class StrategiesApiController : Controller
    {
        private readonly StrategiesControllerUtils utils;

        public StrategiesApiController(StrategiesControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpPost("{name}/{version}")]
        public async Task<IActionResult> Get(string name, string version)
        {
            var result = await utils.Get(name, version);

            var data = result.Model?.Config?.Select(c => new Tmp() { Key = c.Key, Value = c.Value }).ToList() ?? new List<Tmp>();

            return Json(new { result = data, count = data.Count });
        }
        //public async Task<List<Tmp>> Get(string name, string version)
        //{
        //    var result = await utils.Get(name, version);

        //    return result.Model?.Config?.Select(c=>new Tmp() { Key = c.Key, Value = c.Value }).ToList() ?? new List<Tmp>();
        //}
    }

    public class Tmp
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
