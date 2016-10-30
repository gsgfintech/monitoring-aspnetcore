using StratedgemeMonitor.AspNetCore.Models;
using System.Collections.Generic;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class TradeEnginesListViewModel
    {
        public List<TradeEngineModel> TradeEngines { get; set; }

        public TradeEngineActionModel Action { get; set; }

        public TradeEnginesListViewModel()
        {
        }

        public TradeEnginesListViewModel(IEnumerable<TradeEngineModel> tradeEngines)
        {
            TradeEngines = new List<TradeEngineModel>(tradeEngines);
        }
    }

    public class TradeEngineActionModel
    {
        public string TradeEngineName { get; set; }
        public string Action { get; set; }
        public string Cross { get; set; }
        public string Strat { get; set; }
    }
}
