using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Controllers.Executions;
using StratedgemeMonitor.Controllers.FXEvents;
using StratedgemeMonitor.Controllers.Orders;
using StratedgemeMonitor.Models.FXEvents;
using StratedgemeMonitor.ViewModels.Alerts;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.ViewComponents
{
    public class ActivityStatsViewComponent : ViewComponent
    {
        private readonly ExecutionsControllerUtils executionsControllerUtils;
        private readonly FXEventsControllerUtils fxEventsControllerUtils;
        private readonly OrdersControllerUtils ordersControllerUtils;

        public ActivityStatsViewComponent(ExecutionsControllerUtils executionsControllerUtils, FXEventsControllerUtils fxEventsControllerUtils, OrdersControllerUtils ordersControllerUtils)
        {
            this.executionsControllerUtils = executionsControllerUtils;
            this.fxEventsControllerUtils = fxEventsControllerUtils;
            this.ordersControllerUtils = ordersControllerUtils;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int activeOrdersCount = await ordersControllerUtils.GetActiveOrdersCount();
            int inactiveOrdersCount = await ordersControllerUtils.GetInactiveOrdersCount();
            int tradesCount = await executionsControllerUtils.GetTodaysTradesCount();
            (int, FXEventModel) highImpactEvent = await fxEventsControllerUtils.GetHighImpactForTodayCount();

            return View(new ActivityStatsViewModel()
            {
                ActiveOrdersCount = activeOrdersCount,
                InactiveOrdersCount = inactiveOrdersCount,
                NextHighImpactEvent = highImpactEvent.Item2,
                TodaysHighImpactEventsCount = highImpactEvent.Item1,
                TradesCount = tradesCount
            });
        }
    }
}
