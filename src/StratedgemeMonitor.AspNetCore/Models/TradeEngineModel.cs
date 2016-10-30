using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Utils.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.Models
{
    public class TradeEngineModel
    {
        public string Name { get; set; }

        public SystemStatusModel Status { get; set; }

        public TradeEngineConfigModel Config { get; set; }

        public SelectList CrossesList { get; private set; }
        public SelectList TradingCrossesList { get; private set; }
        public SelectList NonTradingCrossesList { get; private set; }
        public SelectList ActiveStratsList { get; private set; }
        public SelectList InactiveStratsList { get; private set; }
        public SelectList TradingStratsList { get; private set; }
        public SelectList NonTradingStratsList { get; private set; }

        public TradeEngineModel(string name, SystemStatusModel status, TradeEngineConfigModel config)
        {
            Name = name;
            Status = status;
            Config = config;

            if (!config.Strats.IsNullOrEmpty())
            {
                CrossesList = new SelectList((new string[1] { "ALL" }).Concat(config.Strats.Select(s => s.Cross.ToString()).OrderBy(c => c)));
                TradingCrossesList = new SelectList((new string[1] { "ALL" }).Concat(config.Strats.Where(s => s.Trading).Select(s => s.Cross.ToString()).OrderBy(c => c)));
                NonTradingCrossesList = new SelectList((new string[1] { "ALL" }).Concat(config.Strats.Where(s => !s.Trading).Select(s => s.Cross.ToString()).OrderBy(c => c)));

                ActiveStratsList = new SelectList(config.Strats.Where(s => s.Active).Select(s => $"{s.Name}-{s.Version}").OrderBy(s => s));
                InactiveStratsList = new SelectList(config.Strats.Where(s => !s.Active).Select(s => $"{s.Name}-{s.Version}").OrderBy(s => s));
                TradingStratsList = new SelectList(config.Strats.Where(s => s.Trading).Select(s => $"{s.Name}-{s.Version}").OrderBy(s => s));
                NonTradingStratsList = new SelectList(config.Strats.Where(s => !s.Trading).Select(s => $"{s.Name}-{s.Version}").OrderBy(s => s));
            }
        }
    }

    public class TradeEngineConfigModel : ISystemConfig
    {
        public string Name { get; set; }
        public string SystemType { get; set; }
        public List<TradeEngineStratConfigModel> Strats { get; set; }
    }

    public class TradeEngineStratConfigModel
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public Cross Cross { get; set; }
        public bool Active { get; set; }
        public bool Trading { get; set; }
    }

    internal static class TradeEngineConfigModelExtensions
    {
        public static TradeEngineConfigModel ToTradeEngineConfigModel(this string json)
        {
            return JsonConvert.DeserializeObject<TradeEngineConfigModel>(json, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
        }

        public static IEnumerable<TradeEngineConfigModel> ToTradeEngineConfigModels(this IEnumerable<string> jsons)
        {
            return jsons?.Select(j => j.ToTradeEngineConfigModel());
        }
    }
}
