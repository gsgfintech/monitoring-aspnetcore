using StratedgemeMonitor.Models.Stratedgeme.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.ViewModels.Stratedgeme.Strategies
{
    public class StrategyDetailsViewModel
    {
        public string Error { get; set; }
        public StrategyModel Strategy { get; private set; }

        public StrategyDetailsViewModel(StrategyModel strategy, string error = null)
        {
            Error = error;
            Strategy = strategy;
        }
    }
}
