using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.AspNetCore.ViewModels;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class DBLoggerController : Controller
    {
        private readonly DBLoggerControllerUtils utils;

        public DBLoggerController(BackendDBLoggerConnector dbLoggerConnector, BackendSystemStatusesConnector statusesConnector, BackendSystemServicesConnector systemServicesConnector, BackendSystemConfigsConnector systemConfigsConnector)
        {
            utils = new DBLoggerControllerUtils(dbLoggerConnector, statusesConnector, systemServicesConnector, systemConfigsConnector);
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DBLoggersListViewModel vm)
        {
            await utils.ExecuteAction(HttpContext.Session, User, vm.Action);

            vm.Action = new DBLoggerActionModel();

            return View(await utils.CreateListViewModel(HttpContext.Session, User));
        }
    }
}
