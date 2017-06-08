namespace StratedgemeMonitor.Models.TradeEngines
{
    public class TradeEngineActionModel
    {
        public string TradeEngineName { get; set; }
        public string Action { get; set; }
        public string Cross { get; set; }
        public string Strat { get; set; }
        public string StratName { get; set; }
        public string StratVersion { get; set; }
    }
}
