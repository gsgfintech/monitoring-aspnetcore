using Capital.GSG.FX.Data.Core.ContractData;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.Models.TradeEngines
{
    public class TradeEngineStratCrossModel
    {
        [Display(Name = "Strategy Name")]
        public string StratName { get; set; }

        [Display(Name = "Strategy Version")]
        public string StratVersion { get; set; }

        public Cross Cross { get; set; }

        public bool IsTrading { get; set; }
    }
}
