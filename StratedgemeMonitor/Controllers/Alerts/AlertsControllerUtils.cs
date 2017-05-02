using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.Models.Alerts;
using StratedgemeMonitor.Models.PnLs;
using StratedgemeMonitor.Models.Systems;
using StratedgemeMonitor.ViewModels.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Alerts
{
    public class AlertsControllerUtils
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<AlertsControllerUtils>();

        private readonly BackendAlertsConnector alertsConnector;
        private readonly BackendPnLsConnector pnlsConnector;
        private readonly BackendSystemStatusesConnector systemStatusesConnector;
        private readonly BackendSystemServicesConnector systemServicesConnector;

        internal DateTime CurrentDay { get; set; }

        public AlertsControllerUtils(BackendAlertsConnector alertsConnector, BackendSystemStatusesConnector systemStatusesConnector, BackendSystemServicesConnector systemServicesConnector, BackendPnLsConnector pnlsConnector)
        {
            this.alertsConnector = alertsConnector;
            this.pnlsConnector = pnlsConnector;
            this.systemStatusesConnector = systemStatusesConnector;
            this.systemServicesConnector = systemServicesConnector;
        }

        internal AlertsViewModel CreateListViewModel(DateTime? day)
        {
            if (!day.HasValue)
                day = DateTime.Today;

            CurrentDay = day.Value;

            return new AlertsViewModel(CurrentDay);
        }

        internal async Task<GenericActionResult> CloseAlert(string id)
        {
            return await alertsConnector.Close(id);
        }

        internal async Task<List<SystemStatusModel>> GetAllSystemStatuses()
        {
            var statuses = await systemStatusesConnector.GetAll();

            return statuses.ToSystemStatusModels();
        }

        internal async Task<List<AlertModel>> GetOpenAlerts()
        {
            logger.Debug("Requesting open alerts");

            var alerts = await alertsConnector.GetByStatus(AlertStatus.OPEN);

            return alerts.ToAlertModels();
        }

        internal async Task<PnLModel> GetPnLForDay(DateTime day)
        {
            var pnlResult = await pnlsConnector.GetForDay(day);

            if (!pnlResult.Success)
                logger.Error(pnlResult.Message);

            return pnlResult.PnLs.ToPnLModel();
        }

        internal async Task<List<AlertModel>> GetClosedAlertsForDay(DateTime day)
        {
            var alerts = await alertsConnector.GetForDay(day);

            return alerts?.Where(b => b.Status == AlertStatus.CLOSED).ToAlertModels();
        }

        internal async Task<AlertModel> Get(string id)
        {
            return (await alertsConnector.GetById(id)).ToAlertModel();
        }

        internal async Task<GenericActionResult> CloseAll()
        {
            return await alertsConnector.CloseAll();
        }

        internal async Task<SystemStatusModel> GetSystemStatus(string systemName)
        {
            return (await systemStatusesConnector.Get(systemName)).ToSystemStatusModel();
        }
    }
}
