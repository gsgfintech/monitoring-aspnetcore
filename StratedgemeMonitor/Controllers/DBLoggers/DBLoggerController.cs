using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.DBLoggers;
using StratedgemeMonitor.ViewModels.DBLoggers;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.DBLoggers
{
    [Authorize]
    public class DBLoggerController : Controller
    {
        private readonly DBLoggerControllerUtils utils;

        public DBLoggerController(DBLoggerControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DBLoggersListViewModel vm)
        {
            await utils.ExecuteAction(vm.Action);

            vm.Action = new DBLoggerActionModel();

            return View(await utils.CreateListViewModel());
        }
    }
}
