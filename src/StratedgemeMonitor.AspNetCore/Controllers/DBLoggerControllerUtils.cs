using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core;
using Microsoft.AspNetCore.Http;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.Utils;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    public class DBLoggerControllerUtils
    {
        private readonly BackendDBLoggerConnector dbLoggerConnector;
        private readonly BackendSystemConfigsConnector systemConfigsConnector;
        private readonly BackendSystemStatusesConnector systemStatusesConnector;
        private readonly BackendSystemServicesConnector systemServicesConnector;

        public DBLoggerControllerUtils(BackendDBLoggerConnector dbLoggerConnector, BackendSystemStatusesConnector systemStatusesConnector, BackendSystemServicesConnector systemServicesConnector, BackendSystemConfigsConnector systemConfigsConnector)
        {
            this.dbLoggerConnector = dbLoggerConnector;
            this.systemStatusesConnector = systemStatusesConnector;
            this.systemServicesConnector = systemServicesConnector;
            this.systemConfigsConnector = systemConfigsConnector;
        }

        internal async Task<DBLoggersListViewModel> CreateListViewModel(ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            var dbLoggers = await LoadDBLoggers(session, user);

            return new DBLoggersListViewModel(dbLoggers);
        }

        private async Task<List<DBLoggerRegionModel>> LoadDBLoggers(ISession session, ClaimsPrincipal user)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            List<DBLoggerSubscriptionStatus> dbLoggerList = await dbLoggerConnector.RequestDBLoggersSubscriptionsStatus(cts.Token);

            if (dbLoggerList.IsNullOrEmpty())
                return new List<DBLoggerRegionModel>();
            else
            {
                Dictionary<Datacenter, DBLoggerRegionModel> dbLoggerRegions = new Dictionary<Datacenter, DBLoggerRegionModel>();

                foreach (var dbLogger in dbLoggerList)
                {
                    SystemStatusModel status = (await systemStatusesConnector.Get(dbLogger.DBLoggerName)).ToSystemStatusModel();

                    if (status != null)
                    {
                        if (!dbLoggerRegions.ContainsKey(status.Datacenter))
                            dbLoggerRegions[status.Datacenter] = new DBLoggerRegionModel(status.Datacenter);

                        dbLoggerRegions[status.Datacenter].DBLoggers.Add(new DBLoggerModel(dbLogger.DBLoggerName, status, dbLogger));
                    }
                }

                return dbLoggerRegions.Values.ToList();
            }
        }

        internal async Task<GenericActionResult> ExecuteAction(ISession session, ClaimsPrincipal user, DBLoggerActionModel actionModel)
        {
            DBLoggerControllerActionValue? action;
            DBLoggerControllerActionValue parsedAction;

            if (!string.IsNullOrEmpty(actionModel.Action) && Enum.TryParse(actionModel.Action, out parsedAction))
                action = parsedAction;
            else
                action = null;

            if (action.HasValue && !string.IsNullOrEmpty(actionModel.DBLoggerName))
            {
                switch (action.Value)
                {
                    case DBLoggerControllerActionValue.Subscribe:
                        if (!actionModel.Crosses.IsNullOrEmpty()) // TODO: use result, handle errors
                            return await Subscribe(actionModel.DBLoggerName, actionModel.Crosses, session, user);
                        break;
                    case DBLoggerControllerActionValue.Unsubscribe:
                        if (!actionModel.Crosses.IsNullOrEmpty()) // TODO: use result, handle errors
                            return await Unsubscribe(actionModel.DBLoggerName, actionModel.Crosses, session, user);
                        break;
                    default:
                        break;
                }
            }

            return null;
        }

        private async Task<GenericActionResult> Subscribe(string dbLoggerName, IEnumerable<Cross> crosses, ISession session, ClaimsPrincipal user)
        {
            if (dbLoggerConnector == null)
                throw new ArgumentNullException(nameof(dbLoggerConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            return await dbLoggerConnector.SubscribePairs(dbLoggerName, crosses.ToArray(), cts.Token);
        }

        private async Task<GenericActionResult> Unsubscribe(string dbLoggerName, IEnumerable<Cross> crosses, ISession session, ClaimsPrincipal user)
        {
            if (dbLoggerConnector == null)
                throw new ArgumentNullException(nameof(dbLoggerConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            return await dbLoggerConnector.UnsubscribePairs(dbLoggerName, crosses.ToArray(), cts.Token);
        }

        private enum DBLoggerControllerActionValue
        {
            Subscribe,
            Unsubscribe
        }
    }
}
