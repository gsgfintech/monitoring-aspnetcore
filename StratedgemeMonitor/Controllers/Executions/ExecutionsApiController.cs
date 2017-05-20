using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.Executions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Executions
{
    [Authorize]
    [Route("api/executions")]
    public class ExecutionsApiController : Controller
    {
        private readonly ExecutionsControllerUtils utils;

        public ExecutionsApiController(ExecutionsControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpPost("{day}")]
        public async Task<IActionResult> GetTradesForDay(DateTime day)
        {
            var result = (await utils.GetExecutions(day)) ?? new List<ExecutionModel>();

            return Json(new { result = result, count = result.Count });
        }
    }
}
