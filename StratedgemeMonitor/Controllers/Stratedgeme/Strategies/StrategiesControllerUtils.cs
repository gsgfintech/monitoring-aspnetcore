using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.Models.Stratedgeme.Strategy;
using StratedgemeMonitor.ViewModels.Stratedgeme.Strategies;
using System;
using System.Collections.Concurrent;
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
        private readonly string appEndpoint;

        private ConcurrentDictionary<(string Name, string Version), StrategyModel> strats = new ConcurrentDictionary<(string Name, string Version), StrategyModel>();

        public StrategiesControllerUtils(BackendStrategiesConnector strategyConnector, string appEndpoint)
        {
            this.strategyConnector = strategyConnector;
            this.appEndpoint = appEndpoint;
        }

        internal async Task<StrategyDetailsViewModel> Details(string name, string version)
        {
            var result = await Get(name, version);

            if (result.Success)
                return new StrategyDetailsViewModel(result.Model);
            else
                return new StrategyDetailsViewModel(null, result.Message);
        }

        internal async Task<StrategyEditViewModel> Edit(string name, string version)
        {
            var result = await Get(name, version);

            return new StrategyEditViewModel(result.Model, appEndpoint);
        }

        internal async Task<StrategyDetailsViewModel> DoEdit(StrategyModel strategy)
        {
            var result = await AddOrUpdate(strategy);

            if (result.Success)
                return new StrategyDetailsViewModel(strategy);
            else
                return new StrategyDetailsViewModel(null, result.Message);
        }

        internal async Task<(bool Success, string Message, StrategyModel Model)> Get(string name, string version)
        {
            if (strats.TryGetValue((name, version), out StrategyModel strat))
                return (true, "Loaded from cache", strat);
            else
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                var result = await strategyConnector.Get(name, version, cts.Token);

                if (result.Success && result.Strat != null)
                {
                    var model = result.Strat.ToStrategyModel();

                    strats.TryAdd((name, version), model);

                    return (true, "Loaded from backend", model);
                }
                else
                    return (result.Success, result.Message, result.Strat.ToStrategyModel());
            }
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
            strats.AddOrUpdate((strat.Name, strat.Version), strat, (key, oldValue) =>
            {
                strat.Config = oldValue.Config;

                return strat;
            });

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            return await strategyConnector.AddOrUpdate(strat.ToStrat(), cts.Token);
        }

        private async Task<(bool Success, string Message)> AddOrUpdateConfig(StrategyModel strat, List<ConfigParamModel> config)
        {
            strat.Config = config;

            strats.AddOrUpdate((strat.Name, strat.Version), strat, (key, oldValue) =>
            {
                oldValue.Config = config;
                return oldValue;
            });

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            return await strategyConnector.AddOrUpdateConfig(strat.Name, strat.Version, config.Where(p => p.Key != "StratName" && p.Key != "StratVersion").ToDictionary(p => p.Key.StartsWith("Param") ? p.Key : $"Param{p.Key}", p => p.Value), cts.Token);
        }

        internal async Task<(bool Success, string Message)> AddConfigParam(string name, string version, ConfigParamModel param)
        {
            var getResult = await Get(name, version);

            if (!getResult.Success || getResult.Model == null)
                return (false, $"Cannot update config of unknown strat {name}-{version}");

            var config = getResult.Model.Config ?? new List<ConfigParamModel>();

            config.Add(param);

            return await AddOrUpdateConfig(getResult.Model, config);
        }

        internal async Task<(bool Success, string Message)> UpdateConfigParam(string name, string version, ConfigParamModel updatedParam)
        {
            var getResult = await Get(name, version);

            if (!getResult.Success || getResult.Model == null)
                return (false, $"Cannot update config of unknown strat {name}-{version}");

            var config = getResult.Model.Config ?? new List<ConfigParamModel>();

            var param = config.FirstOrDefault(p => p.Key == updatedParam.Key || p.Key == $"Param{updatedParam.Key}");

            if (param != null)
                param.Value = updatedParam.Value;

            return await AddOrUpdateConfig(getResult.Model, config);
        }

        internal async Task<(bool Success, string Message)> DeleteConfigParam(string name, string version, string key)
        {
            var getResult = await Get(name, version);

            if (!getResult.Success || getResult.Model == null)
                return (false, $"Cannot update config of unknown strat {name}-{version}");

            var config = getResult.Model.Config ?? new List<ConfigParamModel>();

            var param = config.FirstOrDefault(p => p.Key == key || p.Key == $"Param{key}");

            if (param != null)
                config.Remove(param);

            return await AddOrUpdateConfig(getResult.Model, config);
        }

        internal async Task<(bool Success, string Message)> Delete(string name, string version)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            return await strategyConnector.Delete(name, version, cts.Token);
        }
    }
}
