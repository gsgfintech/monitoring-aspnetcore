using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.Models.Systems;
using StratedgemeMonitor.Models.TradeEngines;
using StratedgemeMonitor.ViewModels.TradeEngines;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.TradeEngines
{
    public class TradeEngineControllerUtils
    {
        private readonly BackendTradeEngineConnector tradeEngineConnector;
        private readonly BackendSystemConfigsConnector systemConfigsConnector;
        private readonly BackendSystemStatusesConnector systemStatusesConnector;
        private readonly BackendSystemServicesConnector systemServicesConnector;

        public TradeEngineControllerUtils(BackendTradeEngineConnector tradeEngineConnector, BackendSystemStatusesConnector systemStatusesConnector, BackendSystemServicesConnector systemServicesConnector, BackendSystemConfigsConnector systemConfigsConnector)
        {
            this.tradeEngineConnector = tradeEngineConnector;
            this.systemStatusesConnector = systemStatusesConnector;
            this.systemServicesConnector = systemServicesConnector;
            this.systemConfigsConnector = systemConfigsConnector;
        }

        internal async Task<TradeEnginesListViewModel> CreateListViewModel()
        {
            Dictionary<string, TradeEngineModel> tradeEngines = await LoadTradeEngines();

            return new TradeEnginesListViewModel(tradeEngines.Values);
        }

        private async Task<Dictionary<string, TradeEngineModel>> LoadTradeEngines()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            List<TradeEngineTradingStatus> enginesList = await tradeEngineConnector.RequestTradeEnginesTradingStatus(cts.Token);

            if (enginesList.IsNullOrEmpty())
                return new Dictionary<string, TradeEngineModel>();
            else
            {
                Dictionary<string, TradeEngineModel> tradeEngines = new Dictionary<string, TradeEngineModel>();

                foreach (var tradeEngineTradingStatus in enginesList)
                {
                    SystemStatusModel status = (await systemStatusesConnector.Get(tradeEngineTradingStatus.EngineName)).ToSystemStatusModel();
                    tradeEngines.Add(tradeEngineTradingStatus.EngineName, new TradeEngineModel(tradeEngineTradingStatus.EngineName, status, tradeEngineTradingStatus));
                }

                return tradeEngines;
            }
        }

        internal async Task<GenericActionResult> ExecuteAction(TradeEngineActionModel actionModel)
        {
            TradeEngineControllerActionValue? action;
            TradeEngineControllerActionValue parsedAction;

            if (!string.IsNullOrEmpty(actionModel.Action) && Enum.TryParse(actionModel.Action, out parsedAction))
                action = parsedAction;
            else
                action = null;

            if (action.HasValue && !string.IsNullOrEmpty(actionModel.TradeEngineName))
            {
                switch (action.Value)
                {
                    case TradeEngineControllerActionValue.StartTrading:
                        if (!string.IsNullOrEmpty(actionModel.Cross)) // TODO: use result, handle errors
                            return await StartTrading(actionModel.TradeEngineName, actionModel.Cross);
                        break;
                    case TradeEngineControllerActionValue.StopTrading:
                        if (!string.IsNullOrEmpty(actionModel.Cross)) // TODO: use result, handle errors
                            return await StopTrading(actionModel.TradeEngineName, actionModel.Cross);
                        break;
                    case TradeEngineControllerActionValue.ClosePosition:
                        if (!string.IsNullOrEmpty(actionModel.Cross)) // TODO: use result, handle errors
                            return await ClosePosition(actionModel.TradeEngineName, actionModel.Cross);
                        break;
                    case TradeEngineControllerActionValue.CancelOrders:
                        if (!string.IsNullOrEmpty(actionModel.Cross)) // TODO: use result, handle errors
                            return await CancelOrders(actionModel.TradeEngineName, actionModel.Cross);
                        break;
                    case TradeEngineControllerActionValue.ActivateStrategy:
                        if (!string.IsNullOrEmpty(actionModel.Strat)) // TODO: use result, handle errors
                            return await ActivateStrategy(actionModel.TradeEngineName, actionModel.Strat);
                        break;
                    case TradeEngineControllerActionValue.DeactivateStrategy:
                        if (!string.IsNullOrEmpty(actionModel.Strat)) // TODO: use result, handle errors
                            return await DeactivateStrategy(actionModel.TradeEngineName, actionModel.Strat);
                        break;
                    case TradeEngineControllerActionValue.StartTradingStrategy:
                        if (!string.IsNullOrEmpty(actionModel.Strat)) // TODO: use result, handle errors
                            return await StartTradingStrategy(actionModel.TradeEngineName, actionModel.Strat);
                        break;
                    case TradeEngineControllerActionValue.StopTradingStrategy:
                        if (!string.IsNullOrEmpty(actionModel.Strat)) // TODO: use result, handle errors
                            return await StopTradingStrategy(actionModel.TradeEngineName, actionModel.Strat);
                        break;
                    default:
                        break;
                }
            }

            return null;
        }

        private async Task<GenericActionResult> StartTrading(string tradeEngineName, string cross)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            return await tradeEngineConnector.StartTrading(tradeEngineName, cross, cts.Token);
        }

        private async Task<GenericActionResult> StopTrading(string tradeEngineName, string cross)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            return await tradeEngineConnector.StopTrading(tradeEngineName, cross, cts.Token);
        }

        private async Task<GenericActionResult> ClosePosition(string tradeEngineName, string cross)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            return await tradeEngineConnector.ClosePosition(tradeEngineName, cross, cts.Token);
        }

        private async Task<GenericActionResult> CancelOrders(string tradeEngineName, string cross)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            return await tradeEngineConnector.CancelOrders(tradeEngineName, cross, cts.Token);
        }

        private async Task<GenericActionResult> ActivateStrategy(string tradeEngineName, string strat)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            string stratName = strat.Split('-')[0];
            string stratVersion = strat.Split('-')[0];

            return await tradeEngineConnector.ActivateStrategy(tradeEngineName, stratName, stratVersion, cts.Token);
        }

        private async Task<GenericActionResult> DeactivateStrategy(string tradeEngineName, string strat)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            string stratName = strat.Split('-')[0];
            string stratVersion = strat.Split('-')[0];

            return await tradeEngineConnector.DeactivateStrategy(tradeEngineName, stratName, stratVersion, cts.Token);
        }

        private async Task<GenericActionResult> StartTradingStrategy(string tradeEngineName, string strat)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            string stratName = strat.Split('-')[0];
            string stratVersion = strat.Split('-')[0];

            return await tradeEngineConnector.StartTradingStrategy(tradeEngineName, stratName, stratVersion, cts.Token);
        }

        private async Task<GenericActionResult> StopTradingStrategy(string tradeEngineName, string strat)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            string stratName = strat.Split('-')[0];
            string stratVersion = strat.Split('-')[0];

            return await tradeEngineConnector.StopTradingStrategy(tradeEngineName, stratName, stratVersion, cts.Token);
        }

        private enum TradeEngineControllerActionValue
        {
            StartTrading,
            StopTrading,
            ClosePosition,
            CancelOrders,
            ActivateStrategy,
            DeactivateStrategy,
            StartTradingStrategy,
            StopTradingStrategy
        }
    }
}
