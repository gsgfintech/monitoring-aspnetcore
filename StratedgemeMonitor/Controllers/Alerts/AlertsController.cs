using Capital.GSG.FX.Data.Core.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.Alerts;
using StratedgemeMonitor.Models.Systems;
using StratedgemeMonitor.ViewModels.Alerts;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Alerts
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
            return View(utils.CreateListViewModel(day));
        }

        public void ChangeDay(DateTime? day)
        {
            if (day.HasValue)
                utils.CurrentDay = day.Value;
        }

        public async Task<IActionResult> Details(string id)
        {
            AlertModel alert = await utils.Get(id);

            if (alert != null)
                return View(alert);
            else
                return View("Error");
        }

        public async Task<IActionResult> SystemDetails(string systemName)
        {
            SystemStatusModel status = await utils.GetSystemStatus(systemName);

            if (status != null)
                return View(status);
            else
                return View("Error");
        }

        public async Task<GenericActionResult> CloseAlert(string id)
        {
            return await utils.CloseAlert(id);
        }

        public async Task<GenericActionResult> CloseAllAlerts()
        {
            return await utils.CloseAll();
        }

        public IActionResult PnlTableViewComponent()
        {
            return ViewComponent("PnlTable", new { day = utils.CurrentDay });
        }

        public IActionResult SystemsListViewComponent()
        {
            return ViewComponent("SystemsList");
        }

        public IActionResult RefreshOpenAlerts()
        {
            return ViewComponent("AlertsList", new { listType = AlertsListType.Open, day = utils.CurrentDay });
        }

        public IActionResult RefreshClosedAlerts()
        {
            return ViewComponent("AlertsList", new { listType = AlertsListType.Closed, day = utils.CurrentDay });
        }

        public IActionResult RefreshActivityStats()
        {
            return ViewComponent("ActivityStats");
        }
    }
}
