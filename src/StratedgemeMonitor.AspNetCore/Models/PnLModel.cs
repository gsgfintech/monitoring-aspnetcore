using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.Models
{
    public class PnLModel
    {
        [Display(Name = "Last Update (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss zzz}")]
        public DateTimeOffset LastUpdate { get; set; }

        public List<PnLPerCrossModel> PerCrossPnLs { get; set; } = new List<PnLPerCrossModel>();

        [Display(Name = "Fees (USD)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalFees { get; set; }

        [Display(Name = "RG PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalGrossRealized { get; set; }

        [Display(Name = "UG PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalGrossUnrealized { get; set; }

        [Display(Name = "RN PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalNetRealized { get; set; }

        [Display(Name = "Trades Cnt")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalTradesCount { get; set; }
    }

    public class PnLPerCrossModel
    {
        [Display(Name = "Pair")]
        public Cross Cross { get; set; }

        [Display(Name = "Position")]
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

        [Display(Name = "Trades Cnt")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TradesCount { get; set; }
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
                PositionSize = pnl.PositionSize / 1000,
                TotalFees = pnl.TotalFees,
                TradesCount = pnl.TradesCount
            };
        }

        private static List<PnLPerCrossModel> ToPnLPerCrossModels(this IEnumerable<PnLPerCross> pnls)
        {
            return pnls?.Select(p => p.ToPnLPerCrossModel()).ToList();
        }

        public static PnLModel ToPnLModel(this PnL pnl)
        {
            if (pnl == null)
                return new PnLModel();

            return new PnLModel()
            {
                LastUpdate = pnl.LastUpdate.ToOffset(TimeSpan.FromHours(8)),
                PerCrossPnLs = pnl.PerCrossPnLs.ToPnLPerCrossModels(),
                TotalFees = pnl.TotalFees,
                TotalGrossRealized = pnl.TotalGrossRealized,
                TotalGrossUnrealized = pnl.TotalGrossUnrealized,
                TotalNetRealized = pnl.TotalNetRealized,
                TotalTradesCount = pnl.TotalTradesCount
            };
        }
    }
}
