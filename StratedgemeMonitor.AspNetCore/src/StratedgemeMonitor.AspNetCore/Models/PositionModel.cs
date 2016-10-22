using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.Models
{
    public class PositionModel
    {
        [Display(Name = "Avg Cost")]
        public double AverageCost { get; set; }

        public Broker Broker { get; set; }

        public Cross Cross { get; set; }

        [Display(Name = "Last Update (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset LastUpdate { get; set; }

        [Display(Name = "Market Price")]
        public double? MarketPrice { get; set; }

        [Display(Name = "Market Value")]
        public double? MarketValue { get; set; }

        [Display(Name = "Quantity")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double PositionQuantity { get; set; }

        [Display(Name = "Realized PnL")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? RealizedPnL { get; set; }

        [Display(Name = "Realized PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? RealizedPnlUsd { get; set; }

        [Display(Name = "Unrealized PnL")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? UnrealizedPnL { get; set; }

        [Display(Name = "Unrealized PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? UnrealizedPnlUsd { get; set; }
    }

    internal static class PositionModelExtensions
    {
        public static PositionModel ToPositionModel(this Position position)
        {
            if (position == null)
                return null;

            return new PositionModel()
            {
                AverageCost = position.AverageCost,
                Broker = position.Broker,
                Cross = position.Cross,
                LastUpdate = position.LastUpdate,
                MarketPrice = position.MarketPrice,
                MarketValue = position.MarketValue,
                PositionQuantity = position.PositionQuantity,
                RealizedPnL = position.RealizedPnL,
                RealizedPnlUsd = position.RealizedPnlUsd,
                UnrealizedPnL = position.UnrealizedPnL,
                UnrealizedPnlUsd = position.UnrealizedPnlUsd
            };
        }

        public static List<PositionModel> ToPositionModels(this IEnumerable<Position> positions)
        {
            return positions?.Select(p => p.ToPositionModel()).ToList();
        }

        public static Position ToPosition(this PositionModel position)
        {
            if (position == null)
                return null;

            return new Position()
            {
                AverageCost = position.AverageCost,
                Broker = position.Broker,
                Cross = position.Cross,
                LastUpdate = position.LastUpdate,
                MarketPrice = position.MarketPrice,
                MarketValue = position.MarketValue,
                PositionQuantity = position.PositionQuantity,
                RealizedPnL = position.RealizedPnL,
                RealizedPnlUsd = position.RealizedPnlUsd,
                UnrealizedPnL = position.UnrealizedPnL,
                UnrealizedPnlUsd = position.UnrealizedPnlUsd
            };
        }

        public static List<Position> ToPositions(this IEnumerable<PositionModel> positions)
        {
            return positions?.Select(p => p.ToPosition()).ToList();
        }
    }
}
