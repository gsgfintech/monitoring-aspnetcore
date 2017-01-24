using Capital.GSG.FX.Data.Core.Strategy;
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

        public List<TradeEngineConfigStrategy> Strats { get; set; }

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
            Strats = config.Strats;

            if (!Strats.IsNullOrEmpty())
            {
                CrossesList = new SelectList((new string[1] { "ALL" }).Concat(Strats.Select(s => s.Cross.ToString()).OrderBy(c => c)));
                TradingCrossesList = new SelectList((new string[1] { "ALL" }).Concat(Strats.Where(s => s.Trading).Select(s => s.Cross.ToString()).OrderBy(c => c)));
                NonTradingCrossesList = new SelectList((new string[1] { "ALL" }).Concat(Strats.Where(s => !s.Trading).Select(s => s.Cross.ToString()).OrderBy(c => c)));

                ActiveStratsList = new SelectList(Strats.Where(s => s.Active).Select(s => $"{s.Name}-{s.Version}").OrderBy(s => s));
                InactiveStratsList = new SelectList(Strats.Where(s => !s.Active).Select(s => $"{s.Name}-{s.Version}").OrderBy(s => s));
                TradingStratsList = new SelectList(Strats.Where(s => s.Trading).Select(s => $"{s.Name}-{s.Version}").OrderBy(s => s));
                NonTradingStratsList = new SelectList(Strats.Where(s => !s.Trading).Select(s => $"{s.Name}-{s.Version}").OrderBy(s => s));
            }
        }
    }

    public class TradeEngineConfigModel
    {
        public string Name { get; set; }
        public List<TradeEngineConfigStrategy> Strats { get; set; }
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
