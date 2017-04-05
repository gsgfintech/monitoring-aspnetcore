using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.MarketData;
using System;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.Models.FXEvents
{
    public class FXEventModel
    {
        public string Actual { get; set; }
        public Currency Currency { get; set; }
        public string Id { get; set; }
        public string Explanation { get; set; }
        public string Forecast { get; set; }
        public FXEventLevel Level { get; set; }
        public string Previous { get; set; }

        [Display(Name = "Time (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public DateTimeOffset Timestamp { get; set; }

        public string Title { get; set; }
    }
}
