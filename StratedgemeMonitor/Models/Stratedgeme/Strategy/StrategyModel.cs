using Capital.GSG.FX.Data.Core.Strategy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.Models.Stratedgeme.Strategy
{
    public class StrategyModel
    {
        public string Name { get; set; }
        public string Version { get; set; }

        [Display(Name = "DLL Path")]
        public string DllPath { get; set; }

        [Display(Name = "Type Name")]
        public string StratTypeName { get; set; }
        public bool Available { get; set; }
        public string Description { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Parameters")]
        public List<ConfigParamModel> Config { get; set; }

        [Display(Name = "Crosses Config")]
        public List<CrossConfigModel> CrossesConfig { get; set; }

        public override string ToString()
        {
            return $"{Name}-{Version}";
        }
    }

    public static class StrategyModelExtensions
    {
        public static StrategyModel ToStrategyModel(this Strat strat)
        {
            if (strat == null)
                return null;

            return new StrategyModel()
            {
                Available = strat.Available,
                Config = strat.Config?.Where(p => p.Key != "StratName" && p.Key != "StratVersion").Select(p => new ConfigParamModel() { Key = p.Key.Replace("Param", ""), Value = p.Value }).ToList(),
                CreationDate = strat.CreationDate,
                CrossesConfig = strat.CrossesAndDefaultTicketSizes?.Select(c => new CrossConfigModel() { Cross = c.Key.ToString(), DefaultTicketSize = c.Value }).ToList(),
                Description = strat.Description,
                DllPath = strat.DllPath,
                Name = strat.Name,
                StratTypeName = strat.StratTypeName,
                Version = strat.Version
            };
        }

        public static List<StrategyModel> ToStrategyModels(this IEnumerable<Strat> strats)
        {
            return strats?.Select(s => s.ToStrategyModel()).ToList();
        }

        public static Strat ToStrat(this StrategyModel strat)
        {
            if (strat == null)
                return null;

            return new Strat(strat.Name, strat.Version, strat.DllPath, strat.StratTypeName, strat.Available)
            {
                CreationDate = strat.CreationDate,
                Description = strat.Description
            };
        }

        public static List<Strat> ToStrats(this IEnumerable<StrategyModel> strats)
        {
            return strats?.Select(s => s.ToStrat()).ToList();
        }
    }
}
