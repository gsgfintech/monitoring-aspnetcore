using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.Models.DBLoggers;
using StratedgemeMonitor.Models.Systems;
using StratedgemeMonitor.ViewModels.DBLoggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.DBLoggers
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

        internal async Task<DBLoggersListViewModel> CreateListViewModel()
        {
            var dbLoggers = await LoadDBLoggers();

            return new DBLoggersListViewModel(dbLoggers);
        }

        private async Task<List<DBLoggerRegionModel>> LoadDBLoggers()
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

        internal async Task<GenericActionResult> ExecuteAction(DBLoggerActionModel actionModel)
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
                            return await Subscribe(actionModel.DBLoggerName, actionModel.Crosses);
                        break;
                    case DBLoggerControllerActionValue.Unsubscribe:
                        if (!actionModel.Crosses.IsNullOrEmpty()) // TODO: use result, handle errors
                            return await Unsubscribe(actionModel.DBLoggerName, actionModel.Crosses);
                        break;
                    default:
                        break;
                }
            }

            return null;
        }

        private async Task<GenericActionResult> Subscribe(string dbLoggerName, IEnumerable<Cross> crosses)
        {
            if (dbLoggerConnector == null)
                throw new ArgumentNullException(nameof(dbLoggerConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            return await dbLoggerConnector.SubscribePairs(dbLoggerName, crosses.ToArray(), cts.Token);
        }

        private async Task<GenericActionResult> Unsubscribe(string dbLoggerName, IEnumerable<Cross> crosses)
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
