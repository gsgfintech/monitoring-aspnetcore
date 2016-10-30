using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.AspNetCore.ViewModels;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class TradeEngineController : Controller
    {
        private readonly TradeEngineControllerUtils utils;

        public TradeEngineController(BackendTradeEngineConnector tradeEngineConnector, BackendSystemStatusesConnector statusesConnector, BackendSystemServicesConnector systemServicesConnector, BackendSystemConfigsConnector systemConfigsConnector)
        {
            utils = new TradeEngineControllerUtils(tradeEngineConnector, statusesConnector, systemServicesConnector, systemConfigsConnector);
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(TradeEnginesListViewModel vm)
        {
            await utils.ExecuteAction(HttpContext.Session, User, vm.Action);

            vm.Action = new TradeEngineActionModel();

            return View(await utils.CreateListViewModel(HttpContext.Session, User));
        }
    }
}
