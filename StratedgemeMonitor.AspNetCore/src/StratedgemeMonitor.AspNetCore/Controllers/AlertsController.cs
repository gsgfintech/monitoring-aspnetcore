using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.AspNetCore.Models;
using Capital.GSG.FX.Data.Core.WebApi;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly AlertsControllerUtils utils;

        public AlertsController(BackendAlertsConnector alertsConnector, BackendSystemStatusesConnector statusesConnector, BackendSystemServicesConnector systemServicesConnector)
        {
            utils = new AlertsControllerUtils(alertsConnector, statusesConnector, systemServicesConnector);
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

        public async Task<IActionResult> SystemStart(string systemName)
        {
            GenericActionResult result = await utils.StartSystem(systemName, HttpContext.Session, User);

            // TODO: use the result

            return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
        }

        public async Task<IActionResult> SystemStop(string systemName)
        {
            GenericActionResult result = await utils.StopSystem(systemName, HttpContext.Session, User);

            // TODO: use the result

            return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
        }

        public async Task<IActionResult> SystemDelete(string systemName)
        {
            bool result = await utils.SystemDelete(systemName, HttpContext.Session, User);

            // TODO: use the result

            return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
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
