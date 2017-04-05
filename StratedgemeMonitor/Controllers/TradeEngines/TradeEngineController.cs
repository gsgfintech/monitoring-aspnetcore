using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.TradeEngines;
using StratedgemeMonitor.ViewModels.TradeEngines;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.TradeEngines
{
    [Authorize]
    public class TradeEngineController : Controller
    {
        private readonly TradeEngineControllerUtils utils;

        public TradeEngineController(TradeEngineControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(TradeEnginesListViewModel vm)
        {
            await utils.ExecuteAction(vm.Action);

            vm.Action = new TradeEngineActionModel();

            return View(await utils.CreateListViewModel());
        }
    }
}
