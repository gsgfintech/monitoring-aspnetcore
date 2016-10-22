using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using StratedgemeMonitor.AspNetCore.Models;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class PositionsController : Controller
    {
        private readonly PositionsControllerUtils utils;

        public PositionsController(BackendPositionsConnector connector)
        {
            utils = new PositionsControllerUtils(connector);
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User));
        }

        public async Task<IActionResult> Details(Broker broker, Cross cross)
        {
            PositionModel position = await utils.Get(broker, cross, HttpContext.Session, User);

            if (position != null)
                return View(position);
            else
                return View("Error");
        }
    }
}
