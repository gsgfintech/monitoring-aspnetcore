using StratedgemeMonitor.Models.Stratedgeme.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.ViewModels.Stratedgeme.Strategies
{
    public class StrategiesListViewModel
    {
        public string Error { get; private set; }
        public List<StrategyModel> Strategies { get; private set; }

        public StrategiesListViewModel(List<StrategyModel> strategies, string error = null)
        {
            Error = error;
            Strategies = strategies;
        }
    }
}
