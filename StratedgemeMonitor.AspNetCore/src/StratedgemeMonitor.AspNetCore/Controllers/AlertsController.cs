using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.AspNetCore.Models;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly AlertsControllerUtils utils;

        public AlertsController(BackendAlertsConnector connector)
        {
            utils = new AlertsControllerUtils(connector);
        }

        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User, day));
        }

        public async Task<IActionResult> Details(string id)
        {
            AlertModel bulletin = await utils.Get(id, HttpContext.Session, User);

            if (bulletin != null)
                return View(bulletin);
            else
                return View("Error");
        }

        public async Task<IActionResult> Close(string id)
        {
            bool result = await utils.Close(id, HttpContext.Session, User);

            if (result)
                return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
            else
                return View("Error");
        }

        public async Task<IActionResult> CloseAll()
        {
            bool result = await utils.CloseAll(HttpContext.Session, User);

            if (result)
                return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
            else
                return View("Error");
        }
    }
}
