using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.Accounts;
using StratedgemeMonitor.Models.Positions;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Positions
{
    [Authorize]
    public class PositionsController : Controller
    {
        private readonly PositionsControllerUtils utils;

        public PositionsController(PositionsControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel());
        }

        public async Task<IActionResult> Details(Broker broker, string account, Cross cross)
        {
            PositionModel position = await utils.Get(broker, account, cross);

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
