using StratedgemeMonitor.Models.Stratedgeme.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.ViewModels.Stratedgeme.Strategies
{
    public class StrategyEditViewModel
    {
        public StrategyModel Strategy { get; private set; }
        public List<KeyValuePair<string, string>> Config { get; set; }

        public StrategyEditViewModel(StrategyModel strategy)
        {
            Strategy = strategy;

            Config = strategy.Config.ToList();
        }
    }
}
