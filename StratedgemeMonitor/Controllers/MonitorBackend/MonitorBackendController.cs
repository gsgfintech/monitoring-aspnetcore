using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.MonitorBackend
{
    [Authorize]
    public class MonitorBackendController : Controller
    {
        private readonly MonitorBackendControllerUtils utils;

        public MonitorBackendController(MonitorBackendControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateMonitorBackendModel());
        }

        public IActionResult ResetTradesDictPartial()
        {
            return PartialView("ResetTradesDictPartial", utils.CreateResetTradesDictModel());
        }
    }
}
