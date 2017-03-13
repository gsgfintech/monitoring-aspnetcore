﻿using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.AspNetCore.ControllerUtils;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.ViewComponents
{
    public class AlertsListViewComponent : ViewComponent
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<AlertsListViewComponent>();

        private readonly AlertsControllerUtils utils;

        public AlertsListViewComponent(AlertsControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IViewComponentResult> InvokeAsync(AlertsListType listType, DateTime day)
        {
            var alerts = listType == AlertsListType.Open ? await utils.GetOpenAlerts() : await utils.GetClosedAlertsForDay(day);

            string header = listType == AlertsListType.Open ? "Open Alerts" : $"Closed Alerts - {day:dd MMM}";

            return View(new AlertsListViewModel(listType, header, alerts));
        }
    }

    public enum AlertsListType
    {
        Open,
        Closed
    }
}
