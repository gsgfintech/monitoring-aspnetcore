using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.Utils;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    internal class AlertsControllerUtils
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<AlertsControllerUtils>();

        private readonly BackendAlertsConnector alertsConnector;
        private readonly BackendPnLsConnector pnlsConnector;
        private readonly BackendSystemStatusesConnector systemStatusesConnector;
        private readonly BackendSystemServicesConnector systemServicesConnector;

        internal DateTime? CurrentDay { get; private set; }

        public AlertsControllerUtils(BackendAlertsConnector alertsConnector, BackendSystemStatusesConnector systemStatusesConnector, BackendSystemServicesConnector systemServicesConnector, BackendPnLsConnector pnlsConnector)
        {
            this.alertsConnector = alertsConnector;
            this.pnlsConnector = pnlsConnector;
            this.systemStatusesConnector = systemStatusesConnector;
            this.systemServicesConnector = systemServicesConnector;
        }

        internal async Task<AlertsListViewModel> CreateListViewModel(ISession session, ClaimsPrincipal user, DateTime? day = null)
        {
            if (!CurrentDay.HasValue)
                CurrentDay = DateTime.Today;

            if (!day.HasValue)
                day = CurrentDay;
            else
                CurrentDay = day;

            List<AlertModel> openAlerts = await GetOpenAlerts(session, user);
            List<AlertModel> closedAlerts = await GetClosedAlertsForDay(day.Value, session, user);
            List<SystemStatusModel> statuses = await GetAllSystemStatuses(session, user);
            PnLModel pnl = await GetPnLForDay(day.Value, session, user);

            return new AlertsListViewModel(day.Value, openAlerts ?? new List<AlertModel>(), closedAlerts ?? new List<AlertModel>(), statuses ?? new List<SystemStatusModel>(), pnl);
        }

        internal async Task<bool> Close(string id, ISession session, ClaimsPrincipal user)
        {
            return await alertsConnector.Close(id);
        }

        private async Task<List<SystemStatusModel>> GetAllSystemStatuses(ISession session, ClaimsPrincipal user)
        {
            var statuses = await systemStatusesConnector.GetAll();

            return statuses.ToSystemStatusModels();
        }

        private async Task<List<AlertModel>> GetOpenAlerts(ISession session, ClaimsPrincipal user)
        {
            logger.Debug("Requesting open alerts");

            var alerts = await alertsConnector.GetByStatus(AlertStatus.OPEN);

            return alerts.ToAlertModels();
        }

        private async Task<PnLModel> GetPnLForDay(DateTime day, ISession session, ClaimsPrincipal user)
        {
            var pnl = await pnlsConnector.GetForDay(day);

            return pnl.ToPnLModel();
        }

        private async Task<List<AlertModel>> GetClosedAlertsForDay(DateTime day, ISession session, ClaimsPrincipal user)
        {
            var alerts = await alertsConnector.GetForDay(day);

            return alerts?.Where(b => b.Status == AlertStatus.CLOSED).ToAlertModels();
        }

        internal async Task<AlertModel> Get(string id, ISession session, ClaimsPrincipal user)
        {
            return (await alertsConnector.GetById(id)).ToAlertModel();
        }

        internal async Task<bool> CloseAll(ISession session, ClaimsPrincipal user)
        {
            return await alertsConnector.CloseAll();
        }

        internal async Task<SystemStatusModel> GetSystemStatus(string systemName, ISession session, ClaimsPrincipal user)
        {
            return (await systemStatusesConnector.Get(systemName)).ToSystemStatusModel();
        }

        internal async Task<GenericActionResult> StartSystem(string systemName, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return await systemServicesConnector.StartService(systemName);
        }

        internal async Task<GenericActionResult> StopSystem(string systemName, ISession session, ClaimsPrincipal user)
        {
            return await systemServicesConnector.StopService(systemName);
        }

        internal async Task<bool> SystemDelete(string systemName, ISession session, ClaimsPrincipal user)
        {
            return await systemStatusesConnector.Delete(systemName);
        }
    }
}
