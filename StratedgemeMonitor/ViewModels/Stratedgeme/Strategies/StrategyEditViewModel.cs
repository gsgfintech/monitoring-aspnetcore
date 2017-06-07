using StratedgemeMonitor.Models.Stratedgeme.Strategy;
using System.Collections.Generic;

namespace StratedgemeMonitor.ViewModels.Stratedgeme.Strategies
{
    public class StrategyEditViewModel
    {
        public StrategyModel Strategy { get; private set; }
        public List<ConfigParamModel> Config { get; set; }
        public List<CrossConfigModel> CrossesConfig { get; set; }

        public string AppEndpoint { get; private set; }

        public StrategyEditViewModel(StrategyModel strategy, string appEndpoint)
        {
            Strategy = strategy;

            Config = strategy.Config;
            CrossesConfig = strategy.CrossesConfig;

            AppEndpoint = appEndpoint;
        }
    }
}
