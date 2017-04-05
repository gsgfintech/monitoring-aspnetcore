using StratedgemeMonitor.Models.TradeEngines;
using System.Collections.Generic;

namespace StratedgemeMonitor.ViewModels.TradeEngines
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
}
