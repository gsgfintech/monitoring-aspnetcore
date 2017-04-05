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

        public PositionsController(BackendPositionsConnector positionsConnector, BackendAccountsConnector accountsConnector)
        {
            utils = new PositionsControllerUtils(positionsConnector, accountsConnector);
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel());
        }

        public async Task<IActionResult> Details(Broker broker, Cross cross)
        {
            PositionModel position = await utils.Get(broker, cross);

            if (position != null)
                return View(position);
            else
                return View("Error");
        }

        public async Task<IActionResult> AccountDetails(Broker broker, string accountName)
        {
            AccountModel account = await utils.GetAccount(broker, accountName);

            if (account != null)
                return View(account);
            else
                return View("Error");
        }
    }
}
