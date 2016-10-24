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

        public AlertsController(BackendAlertsConnector alertsConnector, BackendSystemStatusesConnector statusesConnector)
        {
            utils = new AlertsControllerUtils(alertsConnector, statusesConnector);
        }

        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User, day));
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
