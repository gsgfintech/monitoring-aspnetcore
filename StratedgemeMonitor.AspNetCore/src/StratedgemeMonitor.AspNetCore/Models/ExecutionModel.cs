using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.ExecutionData;
using Capital.GSG.FX.Data.Core.OrderData;
using System;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.AspNetCore.Models
{
    public class ExecutionModel
    {
        public Cross Cross { get; set; }

        [Display(Name = "Time (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTimeOffset ExecutionTime { get; set; }

        public string Id { get; set; }

        [Display(Name = "Origin")]
        public OrderOrigin OrderOrigin { get; set; }

        public double Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}K")]
        public int Quantity { get; set; }

        [Display(Name = "PnL (USD)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? RealizedPnlUsd { get; set; }

        public ExecutionSide Side { get; set; }

        [Display(Name = "Duration")]
        public string TradeDuration { get; set; }

        public int OrderId { get; set; }

        public int ClientId { get; set; }

        public string Exchange { get; set; }

        public string AccountNumber { get; set; }

        public int PermanentID { get; set; }

        public string ClientOrderRef { get; set; }

        public double? Commission { get; set; }

        public Currency? CommissionCcy { get; set; }

        public double? CommissionUsd { get; set; }

        public double? RealizedPnL { get; set; }

        public double? RealizedPnlPips { get; set; }

        public string Strategy { get; set; }

        public override string ToString()
        {
            return $"{Side} {Quantity}K {Cross} @ {Price}";
        }
    }
}
