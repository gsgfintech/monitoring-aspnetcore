using Capital.GSG.FX.Data.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers.Systems
{
    [Route("api/systems")]
    public class SystemsApiController : Controller
    {
        private readonly SystemsControllerUtils utils;

        public SystemsApiController(SystemsControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpGet("start/{systemName}")]
        public async Task<GenericActionResult> SystemStart(string systemName)
        {
            return await utils.StartSystem(systemName);
        }

        [HttpGet("stop/{systemName}")]
        public async Task<GenericActionResult> SystemStop(string systemName)
        {
            return await utils.StopSystem(systemName);
        }

        [HttpDelete("{systemName}")]
        public async Task<GenericActionResult> SystemDelete(string systemName)
        {
            return await utils.SystemDelete(systemName);
        }
    }
}
