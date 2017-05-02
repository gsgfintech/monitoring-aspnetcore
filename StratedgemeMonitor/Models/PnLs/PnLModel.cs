using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Utils.Core;
using static StratedgemeMonitor.Utils.FormatUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.Models.PnLs
{
    public class PnLModel
    {
        [Display(Name = "Last Update (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss zzz}")]
        public DateTimeOffset LastUpdate { get; set; }

        public Dictionary<Cross, PnLPerCrossModel> PerCrossPnLs { get; set; }

        [Display(Name = "Fees (USD)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalFees { get; set; }

        [Display(Name = "RG PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalGrossRealized { get; set; }

        [Display(Name = "UG PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalGrossUnrealized { get; set; }

        [Display(Name = "Total Net")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalNet => TotalNetRealized + TotalGrossUnrealized;

        [Display(Name = "RN PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalNetRealized { get; set; }

        [Display(Name = "Trades Cnt")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalTradesCount { get; set; }

        [Display(Name = "UP PnL")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public double TotalPipsUnrealized => (!PerCrossPnLs.IsNullOrEmpty() && PerCrossPnLs.Values.Count(p => p != null) > 0) ? PerCrossPnLs.Values.Where(p => p != null).Select(p => p.PipsUnrealized).Sum() : 0;
    }

    public class PnLPerCrossModel
    {
        [Display(Name = "Pair")]
        public Cross Cross { get; set; }

        [Display(Name = "Pos")]
        [DisplayFormat(DataFormatString = "{0:N0}K")]
        public double PositionSize { get; set; }

        [Display(Name = "RG PnL")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double GrossRealized { get; set; }

        [Display(Name = "UG PnL")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double GrossUnrealized { get; set; }

        [Display(Name = "RN PnL")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double NetRealized { get; set; }

        [Display(Name = "Fees")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalFees { get; set; }

        [Display(Name = "Trades")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TradesCount { get; set; }

        [Display(Name = "PO (HKT)")]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}")]
        public DateTimeOffset? PositionOpenTime { get; set; }

        [Display(Name = "Pos Duration")]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm\:ss}")]
        public TimeSpan? PositionOpenDuration => (PositionOpenTime.HasValue) ? DateTimeOffset.Now.Subtract(PositionOpenTime.Value) : (TimeSpan?)null;

        [Display(Name = "PO Price")]
        public string PositionOpenPrice { get; set; }

        [Display(Name = "UP PnL")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public double PipsUnrealized { get; set; }
    }

    internal static class PnLModelExtensions
    {
        private static PnLPerCrossModel ToPnLPerCrossModel(this PnLPerCross pnl)
        {
            if (pnl == null)
                return null;

            return new PnLPerCrossModel()
            {
                Cross = pnl.Cross,
                GrossRealized = pnl.GrossRealized,
                GrossUnrealized = pnl.GrossUnrealized,
                NetRealized = pnl.NetRealized,
                PipsUnrealized = pnl.PipsUnrealized,
                PositionOpenPrice = FormatRate(pnl.Cross, pnl.PositionOpenPrice),
                PositionOpenTime = pnl.PositionOpenTime.HasValue ? pnl.PositionOpenTime.Value.ToLocalTime() : (DateTimeOffset?)null,
                PositionSize = pnl.PositionSize / 1000,
                TotalFees = pnl.TotalFees,
                TradesCount = pnl.TradesCount
            };
        }

        private static List<PnLPerCrossModel> ToPnLPerCrossModels(this IEnumerable<PnLPerCross> pnls)
        {
            return pnls?.Select(p => p.ToPnLPerCrossModel()).ToList();
        }

        public static PnLModel ToPnLModel(this IEnumerable<PnL> pnls)
        {
            pnls = pnls.Where(p => (p.Broker == Broker.IB) ? !p.Account.Contains("F") : true); // Ignore IB institutional account as they mess up statistics

            if (pnls.IsNullOrEmpty())
                return new PnLModel();

            // Combine all pnls (temporary solution)
            var combinedPerCross = pnls.Select(p => p.PerCrossPnLs.AsEnumerable()).Aggregate((cur, next) => cur.Concat(next)).GroupBy(p => p.Cross).Select(g => new PnLPerCross()
            {
                Cross = g.Key,
                GrossRealized = g.Select(p => p.GrossRealized).Sum(),
                GrossUnrealized = g.Select(p => p.GrossUnrealized).Sum(),
                PipsUnrealized = g.Select(p => p.PipsUnrealized).Sum(),
                PositionOpenPrice = g.Select(p => p.PositionOpenPrice)?.FirstOrDefault(),
                PositionOpenTime = g.Select(p => p.PositionOpenTime)?.FirstOrDefault(),
                PositionSize = g.Select(p => p.PositionSize).Sum(),
                TotalFees = g.Select(p => p.TotalFees).Sum(),
                TradesCount = g.Select(p => p.TradesCount).Sum() / g.Count() // TODO: find a nicer solution
            }).ToList();

            var combinedPnl = new PnL()
            {
                Account = "Total",
                Broker = Broker.UNKNOWN,
                LastUpdate = pnls.Select(p => p.LastUpdate).Max(),
                PerCrossPnLs = combinedPerCross
            };

            return combinedPnl.ToPnLModel();
        }

        public static PnLModel ToPnLModel(this PnL pnl)
        {
            if (pnl == null)
                return new PnLModel();

            // We want to display the pairs in a specific order
            Dictionary<Cross, PnLPerCrossModel> pnlPerCrossDict = new Dictionary<Cross, PnLPerCrossModel>();
            pnlPerCrossDict.Add(Cross.EURUSD, pnl.PerCrossPnLs.FirstOrDefault(p => p.Cross == Cross.EURUSD).ToPnLPerCrossModel());
            pnlPerCrossDict.Add(Cross.USDJPY, pnl.PerCrossPnLs.FirstOrDefault(p => p.Cross == Cross.USDJPY).ToPnLPerCrossModel());
            pnlPerCrossDict.Add(Cross.GBPUSD, pnl.PerCrossPnLs.FirstOrDefault(p => p.Cross == Cross.GBPUSD).ToPnLPerCrossModel());
            pnlPerCrossDict.Add(Cross.AUDUSD, pnl.PerCrossPnLs.FirstOrDefault(p => p.Cross == Cross.AUDUSD).ToPnLPerCrossModel());
            pnlPerCrossDict.Add(Cross.USDCAD, pnl.PerCrossPnLs.FirstOrDefault(p => p.Cross == Cross.USDCAD).ToPnLPerCrossModel());
            pnlPerCrossDict.Add(Cross.USDCHF, pnl.PerCrossPnLs.FirstOrDefault(p => p.Cross == Cross.USDCHF).ToPnLPerCrossModel());
            pnlPerCrossDict.Add(Cross.NZDUSD, pnl.PerCrossPnLs.FirstOrDefault(p => p.Cross == Cross.NZDUSD).ToPnLPerCrossModel());

            return new PnLModel()
            {
                LastUpdate = pnl.LastUpdate.ToLocalTime(),
                PerCrossPnLs = pnlPerCrossDict,
                TotalFees = pnl.TotalFees,
                TotalGrossRealized = pnl.TotalGrossRealized,
                TotalGrossUnrealized = pnl.TotalGrossUnrealized,
                TotalNetRealized = pnl.TotalNetRealized,
                TotalTradesCount = pnl.TotalTradesCount
            };
        }
    }
}
