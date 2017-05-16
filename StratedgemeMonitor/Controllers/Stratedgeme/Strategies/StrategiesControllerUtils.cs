using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.Models.Stratedgeme.Strategy;
using StratedgemeMonitor.ViewModels.Stratedgeme.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Stratedgeme.Strategies
{
    public class StrategiesControllerUtils
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<StrategiesControllerUtils>();

        private readonly BackendStrategiesConnector strategyConnector;

        public StrategiesControllerUtils(BackendStrategiesConnector strategyConnector)
        {
            this.strategyConnector = strategyConnector;
        }

        internal async Task<StrategyDetailsViewModel> Details(string name, string version)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var result = await strategyConnector.Get(name, version, cts.Token);

            if (result.Success)
                return new StrategyDetailsViewModel(result.Strat.ToStrategyModel());
            else
                return new StrategyDetailsViewModel(null, result.Message);
        }

        internal async Task<StrategiesListViewModel> Index()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var result = await strategyConnector.GetAll(cts.Token);

            if (result.Success)
                return new StrategiesListViewModel(result.Strats.ToStrategyModels());
            else
                return new StrategiesListViewModel(null, result.Message);
        }

        internal async Task<StrategiesListViewModel> Available()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var result = await strategyConnector.GetAllAvailable(cts.Token);

            if (result.Success)
                return new StrategiesListViewModel(result.Strats.ToStrategyModels());
            else
                return new StrategiesListViewModel(null, result.Message);
        }

        internal async Task<(bool Success, string Message)> AddOrUpdate(StrategyModel strat)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            return await strategyConnector.AddOrUpdate(strat.ToStrat(), cts.Token);
        }

        internal async Task<(bool Success, string Message)> Delete(string name, string version)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            return await strategyConnector.Delete(name, version, cts.Token);
        }
    }
}
