using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Utils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.Models.Positions
{
    public class PositionModel
    {
        public string Account { get; set; }

        [Display(Name = "Avg Cost")]
        public double AverageCost { get; set; }

        public Broker Broker { get; set; }

        public Cross Cross { get; set; }

        [Display(Name = "Last Update (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset LastUpdate { get; set; }

        [Display(Name = "Quantity")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double PositionQuantity { get; set; }
    }

    internal static class PositionModelExtensions
    {
        public static PositionModel ToPositionModel(this Position position)
        {
            if (position == null)
                return null;

            return new PositionModel()
            {
                Account = position.Account,
                AverageCost = position.AverageCost,
                Broker = position.Broker,
                Cross = position.Cross,
                LastUpdate = position.LastUpdate,
                PositionQuantity = position.PositionQuantity
            };
        }

        public static List<PositionModel> ToPositionModels(this IEnumerable<Position> positions)
        {
            return positions?.Select(p => p.ToPositionModel()).ToList();
        }

        public static Dictionary<string, List<PositionModel>> ToPositionModelsDict(this IEnumerable<Position> positions)
        {
            if (positions.IsNullOrEmpty())
                return new Dictionary<string, List<PositionModel>>();

            positions = positions.Where(p => p.Broker != Broker.UNKNOWN && !string.IsNullOrEmpty(p.Account));

            if (positions.IsNullOrEmpty())
                return new Dictionary<string, List<PositionModel>>();

            var groupings = positions.GroupBy(p => $"{p.Broker}-{p.Account}");

            return groupings.ToDictionary(g => g.Key, g => g.ToPositionModels());
        }

        public static Position ToPosition(this PositionModel position)
        {
            if (position == null)
                return null;

            return new Position()
            {
                Account = position.Account,
                AverageCost = position.AverageCost,
                Broker = position.Broker,
                Cross = position.Cross,
                LastUpdate = position.LastUpdate,
                PositionQuantity = position.PositionQuantity,
            };
        }

        public static List<Position> ToPositions(this IEnumerable<PositionModel> positions)
        {
            return positions?.Select(p => p.ToPosition()).ToList();
        }
    }
}
