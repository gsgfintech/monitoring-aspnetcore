using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.AspNetCore.ControllerUtils;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.ViewComponents
{
    public class PnlTableViewComponent : ViewComponent
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<PnlTableViewComponent>();

        private readonly AlertsControllerUtils utils;

        public PnlTableViewComponent(AlertsControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pnl = await utils.GetPnLForDay();

            return View(pnl);
        }
    }
}
