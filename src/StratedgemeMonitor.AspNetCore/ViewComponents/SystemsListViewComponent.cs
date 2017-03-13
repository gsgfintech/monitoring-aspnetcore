using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Utils.Core;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.AspNetCore.ControllerUtils;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.ViewComponents
{
    public class SystemsListViewComponent : ViewComponent
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<SystemsListViewComponent>();

        private readonly AlertsControllerUtils utils;

        public SystemsListViewComponent(AlertsControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var statuses = await utils.GetAllSystemStatuses();

            var overallStatus = CalculateOverall(statuses);

            return View(new SystemsListViewModel(overallStatus, statuses));
        }

        private SystemStatusLevel? CalculateOverall(IEnumerable<SystemStatusModel> statuses)
        {
            if (statuses.IsNullOrEmpty())
                return null;

            if (statuses.Where(s => !s.IsAlive).Count() > 0)
                return SystemStatusLevel.RED;

            return SystemStatusLevelUtils.CalculateWorstOf(statuses.Select(s => s.OverallStatus ?? SystemStatusLevel.GREEN));
        }
    }
}
