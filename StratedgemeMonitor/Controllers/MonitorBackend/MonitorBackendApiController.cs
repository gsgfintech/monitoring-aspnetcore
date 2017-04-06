using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.MonitorBackend
{
    [Authorize]
    [Route("api/monitorbackend")]
    public class MonitorBackendApiController : Controller
    {
        private readonly MonitorBackendControllerUtils utils;

        public MonitorBackendApiController(MonitorBackendControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpGet("reset-trades-dict")]
        public async Task<(bool,string)> ResetTradesDictionary()
        {
            return await utils.ResetTradesDictionary();
        }
    }
}
