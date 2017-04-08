using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.Controllers.Alerts;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.ViewComponents
{
    public class PnlTableViewComponent : ViewComponent
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<PnlTableViewComponent>();

        private readonly AlertsControllerUtils utils;

        public PnlTableViewComponent(AlertsControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IViewComponentResult> InvokeAsync(DateTime day)
        {
            var pnl = await utils.GetPnLForDay(day);

            return View(pnl);
        }
    }
}
