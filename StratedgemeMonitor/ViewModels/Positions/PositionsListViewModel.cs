using Capital.GSG.FX.Data.Core.ContractData;
using StratedgemeMonitor.Models.Accounts;
using StratedgemeMonitor.Models.Positions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.ViewModels.Positions
{
    public class PositionsListViewModel
    {
        public List<AccountModel> Accounts { get; set; }
        public List<PositionModel> TotalPosition { get; private set; }
        public Dictionary<string, List<PositionModel>> Positions { get; set; }

        public PositionsListViewModel(Dictionary<string, List<PositionModel>> positions, List<AccountModel> accounts)
        {
            Accounts = accounts;
            Positions = positions;

            ComputeTotalPosition(positions);
        }

        private void ComputeTotalPosition(Dictionary<string, List<PositionModel>> positions)
        {
            TotalPosition = positions.Values.Aggregate((cur, next) => cur.Concat(next).ToList()).GroupBy(p => p.Cross).Select(g => new PositionModel()
            {
                Account = "Total",
                AverageCost = Math.Round(g.Select(p => p.AverageCost).Average(), g.Key.GetDecimalsCount()),
                Broker = Capital.GSG.FX.Data.Core.AccountPortfolio.Broker.UNKNOWN,
                Cross = g.Key,
                LastUpdate = g.Select(p => p.LastUpdate).Min(),
                PositionQuantity = g.Select(p => p.PositionQuantity).Sum()
            }).ToList();
        }
    }
}
