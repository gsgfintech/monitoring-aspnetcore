using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Utils.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using StratedgemeMonitor.Models.Systems;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.Models.TradeEngines
{
    public class TradeEngineModel
    {
        public string Name { get; set; }

        public SystemStatusModel Status { get; set; }

        [Display(Name = "Trading Status")]
        public List<TradeEngineStratCrossModel> TradingStatus => !Strats.IsNullOrEmpty() ? Strats.Select(s => !s.Crosses.IsNullOrEmpty() ? s.Crosses.Select(c => new TradeEngineStratCrossModel()
        {
            Cross = c.Cross,
            IsTrading = c.IsTrading,
            StratName = s.Name,
            StratVersion = s.Version
        }) : new List<TradeEngineStratCrossModel>()).Aggregate((cur, next) => cur.Concat(next)).ToList() : new List<TradeEngineStratCrossModel>();

        public bool IsAllTrading => !TradingStatus.IsNullOrEmpty() && TradingStatus.Count(c => !c.IsTrading) == 0;

        public List<TradeEngineTradingStatusStrat> Strats { get; set; }

        public SelectList CrossesList { get; private set; }
        public SelectList TradingCrossesList { get; private set; }
        public SelectList NonTradingCrossesList { get; private set; }
        public SelectList AllStratsList { get; private set; }
        public SelectList ActiveStratsList { get; private set; }
        public SelectList InactiveStratsList { get; private set; }
        public SelectList TradingStratsList { get; private set; }
        public SelectList NonTradingStratsList { get; private set; }

        [Display(Name = "Reset Trading Conn Status")]
        public bool IsTradingConnectionConnected { get; private set; }

        public TradeEngineModel(string name, SystemStatusModel status, TradeEngineTradingStatus tradingStatus)
        {
            IsTradingConnectionConnected = tradingStatus.IsTradingConnectionConnected;
            Name = name;
            Status = status;
            Strats = tradingStatus.Strats;

            if (!TradingStatus.IsNullOrEmpty())
            {
                var allCrosses = TradingStatus.Select(c => c.Cross.ToString()).Distinct();
                var tradingCrosses = TradingStatus.Where(c => c.IsTrading).Select(c => c.Cross.ToString()).Distinct();

                CrossesList = new SelectList((new string[1] { "ALL" }).Concat(allCrosses.OrderBy(c => c)));
                TradingCrossesList = new SelectList((new string[1] { "ALL" }).Concat(tradingCrosses.OrderBy(c => c)));
                NonTradingCrossesList = new SelectList((new string[1] { "ALL" }).Concat(allCrosses.Except(tradingCrosses).OrderBy(c => c)));
            }
            else
            {
                CrossesList = new SelectList(new string[1] { "ALL" });
                TradingCrossesList = new SelectList(new string[1] { "ALL" });
                NonTradingCrossesList = new SelectList(new string[1] { "ALL" });
            }

            if (!TradingStatus.IsNullOrEmpty())
            {
                var allStrats = TradingStatus.Select(c => $"{c.StratName}-{c.StratVersion}").Distinct();
                var tradingStrats = TradingStatus.Where(c => c.IsTrading).Select(s => $"{s.StratName}-{s.StratVersion}").Distinct();

                AllStratsList = new SelectList(allStrats.OrderBy(s => s));
                TradingStratsList = new SelectList(tradingStrats.OrderBy(s => s));
                NonTradingStratsList = new SelectList(allStrats.Except(tradingStrats).OrderBy(s => s));
            }
        }
    }
}
