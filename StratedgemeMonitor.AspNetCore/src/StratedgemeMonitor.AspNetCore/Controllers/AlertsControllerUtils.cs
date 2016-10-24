﻿using Capital.GSG.FX.Data.Core.SystemData;
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
        private readonly BackendSystemStatusesConnector systemStatusesConnector;

        internal DateTime? CurrentDay { get; private set; }

        public AlertsControllerUtils(BackendAlertsConnector alertsConnector, BackendSystemStatusesConnector systemStatusesConnector)
        {
            this.alertsConnector = alertsConnector;
            this.systemStatusesConnector = systemStatusesConnector;
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

            return new AlertsListViewModel(day.Value, openAlerts ?? new List<AlertModel>(), closedAlerts ?? new List<AlertModel>(), statuses ?? new List<Models.SystemStatusModel>());
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
    }
}
