using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StratedgemeMonitor.AspNetCore.Models;
using Capital.GSG.FX.Data.Core.WebApi;
using StratedgemeMonitor.AspNetCore.ControllerUtils;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly AlertsControllerUtils utils;

        public AlertsController(AlertsControllerUtils utils)
        {
            this.utils = utils;
        }

        public IActionResult Index(DateTime? day)
        {
            return View(utils.CreateListViewModel());
        }

        public async Task<IActionResult> Details(string id)
        {
            AlertModel alert = await utils.Get(id, HttpContext.Session, User);

            if (alert != null)
                return View(alert);
            else
                return View("Error");
        }

        public async Task<IActionResult> SystemDetails(string systemName)
        {
            SystemStatusModel status = await utils.GetSystemStatus(systemName, HttpContext.Session, User);

            if (status != null)
                return View(status);
            else
                return View("Error");
        }

        public async Task<IActionResult> SystemStart(string systemName)
        {
            GenericActionResult result = await utils.StartSystem(systemName, HttpContext.Session, User);

            // TODO: use the result

            return View("Index", utils.CreateListViewModel(utils.CurrentDay));
        }

        public async Task<IActionResult> SystemStop(string systemName)
        {
            GenericActionResult result = await utils.StopSystem(systemName, HttpContext.Session, User);

            // TODO: use the result

            return View("Index", utils.CreateListViewModel(utils.CurrentDay));
        }

        public async Task<IActionResult> SystemDelete(string systemName)
        {
            var result = await utils.SystemDelete(systemName, HttpContext.Session, User);

            // TODO: use the result

            return View("Index", utils.CreateListViewModel(utils.CurrentDay));
        }

        public async Task<IActionResult> Close(string id)
        {
            GenericActionResult result = await utils.Close(id, HttpContext.Session, User);

            if (result.Success)
                return View("Index", utils.CreateListViewModel(utils.CurrentDay));
            else
                return View("Error", result.Message);
        }

        public async Task<IActionResult> CloseAll()
        {
            GenericActionResult result = await utils.CloseAll(HttpContext.Session, User);

            if (result.Success)
                return View("Index", utils.CreateListViewModel(utils.CurrentDay));
            else
                return View("Error", result.Message);
        }

        public IActionResult PnlTableViewComponent()
        {
            return ViewComponent("PnlTable");
        }

        public IActionResult SystemsListViewComponent()
        {
            return ViewComponent("SystemsList");
        }
    }
}
