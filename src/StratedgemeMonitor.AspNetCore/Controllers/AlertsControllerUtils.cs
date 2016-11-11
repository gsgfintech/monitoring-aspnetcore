using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Microsoft.AspNetCore.Http;
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
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            var alert = await alertsConnector.GetById(id, accessToken);

            if (alert != null)
            {
                alert.Status = AlertStatus.CLOSED;
                alert.ClosedTimestamp = DateTimeOffset.Now;

                return await alertsConnector.AddOrUpdate(alert, accessToken);
            }
            else
                return false;
        }

        private async Task<List<SystemStatusModel>> GetAllSystemStatuses(ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            var statuses = await systemStatusesConnector.GetAll(accessToken);

            return statuses.ToSystemStatusModels();
        }

        private async Task<List<AlertModel>> GetOpenAlerts(ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            var alerts = await alertsConnector.GetByStatus(AlertStatus.OPEN, accessToken);

            return alerts.ToAlertModels();
        }

        private async Task<PnLModel> GetPnLForDay(DateTime day, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            var pnl = await pnlsConnector.GetForDay(day, accessToken);

            return pnl.ToPnLModel();
        }

        private async Task<List<AlertModel>> GetClosedAlertsForDay(DateTime day, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            var alerts = await alertsConnector.GetForDay(day, accessToken);

            return alerts?.Where(b => b.Status == AlertStatus.CLOSED).ToAlertModels();
        }

        internal async Task<AlertModel> Get(string id, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return (await alertsConnector.GetById(id, accessToken)).ToAlertModel();
        }

        internal async Task<bool> CloseAll(ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return await alertsConnector.CloseAll(accessToken);
        }

        internal async Task<SystemStatusModel> GetSystemStatus(string systemName, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return (await systemStatusesConnector.Get(systemName, accessToken)).ToSystemStatusModel();
        }

        internal async Task<GenericActionResult> StartSystem(string systemName, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return await systemServicesConnector.StartService(systemName, accessToken);
        }

        internal async Task<GenericActionResult> StopSystem(string systemName, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return await systemServicesConnector.StopService(systemName, accessToken);
        }

        internal async Task<bool> SystemDelete(string systemName, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return await systemStatusesConnector.Delete(systemName, accessToken);
        }
    }
}
