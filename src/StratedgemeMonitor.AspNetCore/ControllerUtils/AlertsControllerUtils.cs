﻿using Capital.GSG.FX.Data.Core.AccountPortfolio;
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

namespace StratedgemeMonitor.AspNetCore.ControllerUtils
{
    public class AlertsControllerUtils
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

        internal AlertsViewModel CreateListViewModel(DateTime? day = null)
        {
            if (!CurrentDay.HasValue)
                CurrentDay = DateTime.Today;

            if (!day.HasValue)
                day = CurrentDay;
            else
                CurrentDay = day;

            return new AlertsViewModel(day.Value);
        }

        internal async Task<GenericActionResult> Close(string id, ISession session, ClaimsPrincipal user)
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
            var pnl = await pnlsConnector.GetForDay(day);

            return pnl.ToPnLModel();
        }

        internal async Task<PnLModel> GetPnLForDay()
        {
            return await GetPnLForDay(CurrentDay ?? DateTime.Today);
        }

        internal async Task<List<AlertModel>> GetClosedAlertsForDay(DateTime day)
        {
            var alerts = await alertsConnector.GetForDay(day);

            return alerts?.Where(b => b.Status == AlertStatus.CLOSED).ToAlertModels();
        }

        internal async Task<AlertModel> Get(string id, ISession session, ClaimsPrincipal user)
        {
            return (await alertsConnector.GetById(id)).ToAlertModel();
        }

        internal async Task<GenericActionResult> CloseAll(ISession session, ClaimsPrincipal user)
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

        internal async Task<GenericActionResult> SystemDelete(string systemName, ISession session, ClaimsPrincipal user)
        {
            return await systemStatusesConnector.Delete(systemName);
        }
    }
}
