using Capital.GSG.FX.Data.Core.SystemData;
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

        internal async Task<(bool Success, string Message)> ExecuteAction(TradeEngineActionModel actionModel)
        {
            TradeEngineControllerActionValue? action;

            if (!string.IsNullOrEmpty(actionModel.Action) && Enum.TryParse(actionModel.Action, out TradeEngineControllerActionValue parsedAction))
                action = parsedAction;
            else
                action = null;

            if (action.HasValue && !string.IsNullOrEmpty(actionModel.TradeEngineName))
            {
                switch (action.Value)
                {
                    case TradeEngineControllerActionValue.StartTrading:
                        if (!string.IsNullOrEmpty(actionModel.Cross)) // TODO: use result, handle errors
                            return await StartTrading(actionModel.TradeEngineName, actionModel.StratName, actionModel.StratVersion, actionModel.Cross);
                        break;
                    case TradeEngineControllerActionValue.StopTrading:
                        if (!string.IsNullOrEmpty(actionModel.Cross)) // TODO: use result, handle errors
                            return await StopTrading(actionModel.TradeEngineName, actionModel.StratName, actionModel.StratVersion, actionModel.Cross);
                        break;
                    case TradeEngineControllerActionValue.ClosePosition:
                        if (!string.IsNullOrEmpty(actionModel.Cross)) // TODO: use result, handle errors
                            return await ClosePosition(actionModel.TradeEngineName, actionModel.Cross);
                        break;
                    case TradeEngineControllerActionValue.CancelOrders:
                        if (!string.IsNullOrEmpty(actionModel.Cross)) // TODO: use result, handle errors
                            return await CancelOrders(actionModel.TradeEngineName, actionModel.Cross);
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

            return (false, "Invalid action");
        }

        internal async Task<(bool Success, string Message)> StartTrading(string tradeEngineName, string stratName, string stratVersion, string cross)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            if (!string.IsNullOrEmpty(stratName) && !string.IsNullOrEmpty(stratVersion))
                return await tradeEngineConnector.StartTrading(tradeEngineName, stratName, stratVersion, cross, cts.Token);
            else
                return await tradeEngineConnector.StartTrading(tradeEngineName, cross, cts.Token);
        }

        internal async Task<(bool Success, string Message)> StopTrading(string tradeEngineName, string stratName, string stratVersion, string cross)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            if (!string.IsNullOrEmpty(stratName) && !string.IsNullOrEmpty(stratVersion))
                return await tradeEngineConnector.StopTrading(tradeEngineName, stratName, stratVersion, cross, cts.Token);
            else
                return await tradeEngineConnector.StopTrading(tradeEngineName, cross, cts.Token);
        }

        private async Task<(bool Success, string Message)> ClosePosition(string tradeEngineName, string cross)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            var result = await tradeEngineConnector.ClosePosition(tradeEngineName, cross, cts.Token);

            return (result.Success, result.Message);
        }

        private async Task<(bool Success, string Message)> CancelOrders(string tradeEngineName, string cross)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            var result = await tradeEngineConnector.CancelOrders(tradeEngineName, cross, cts.Token);

            return (result.Success, result.Message);
        }

        private async Task<(bool Success, string Message)> StartTradingStrategy(string tradeEngineName, string strat)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            string stratName = strat.Split('-')[0];
            string stratVersion = strat.Split('-')[1];

            var result = await tradeEngineConnector.StartTradingStrategy(tradeEngineName, stratName, stratVersion, cts.Token);

            return (result.Success, result.Message);
        }

        private async Task<(bool Success, string Message)> StopTradingStrategy(string tradeEngineName, string strat)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            string stratName = strat.Split('-')[0];
            string stratVersion = strat.Split('-')[1];

            var result = await tradeEngineConnector.StopTradingStrategy(tradeEngineName, stratName, stratVersion, cts.Token);

            return (result.Success, result.Message);
        }

        internal async Task<(bool Success, string Message)> RequestStratToResetPositionStatus(string tradeEngineName, string strat)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            string stratName = strat.Split('-')[0];
            string stratVersion = strat.Split('-')[1];

            var result = await tradeEngineConnector.RequestStratToResetPositionStatus(tradeEngineName, stratName, stratVersion, cts.Token);

            return (result?.Success ?? false, result?.Message);
        }

        internal async Task<(bool Success, string Message)> ResetTradingConnectionStatus(string tradeEngineName, bool isConnected)
        {
            if (tradeEngineConnector == null)
                throw new ArgumentNullException(nameof(tradeEngineConnector));

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            var result = await tradeEngineConnector.ResetTradingConnectionStatus(tradeEngineName, isConnected, cts.Token);

            return (result?.Success ?? false, result?.Message);
        }

        private enum TradeEngineControllerActionValue
        {
            StartTrading,
            StopTrading,
            ClosePosition,
            CancelOrders,
            StartTradingStrategy,
            StopTradingStrategy
        }
    }
}
