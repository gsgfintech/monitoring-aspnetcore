using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.OrderData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.Models.Orders
{
    public class OrderModel
    {
        public string Account { get; set; }

        public object AllocationInfo { get; set; }
        public string AllocationInfoStr => AllocationInfo?.ToString();

        public Broker Broker { get; set; }

        [Display(Name = "Pair")]
        public Cross Cross { get; set; }

        [Display(Name = "Fill Price")]
        public double? FillPrice { get; set; }

        [Display(Name = "ID")]
        public int OrderId { get; set; }

        public OrderOrigin Origin { get; set; }

        public long PermanentId { get; set; }

        [Display(Name = "Placed Time (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset? PlacedTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}K")]
        public double Quantity { get; set; }

        public OrderSide Side { get; set; }

        public OrderStatusCode Status { get; set; }

        public OrderType Type { get; set; }

        [Display(Name = "Commission (est.)")]
        public double? EstimatedCommission { get; set; }

        public Currency? EstimatedCommissionCcy { get; set; }

        public List<OrderHistoryPointModel> History { get; set; }

        [Display(Name = "Last Ask")]
        public double? LastAsk { get; set; }

        [Display(Name = "Last Bid")]
        public double? LastBid { get; set; }

        [Display(Name = "Last Mid")]
        public double? LastMid { get; set; }

        [Display(Name = "Last Update (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset? LastUpdateTime { get; set; }

        [Display(Name = "Limit Price")]
        public double? LimitPrice { get; set; }

        public string OurRef { get; set; }

        public int? ParentOrderId { get; set; }

        [Display(Name = "Stop Price")]
        public double? StopPrice { get; set; }

        public string Strategy { get; set; }

        [Display(Name = "TIF")]
        public TimeInForce TimeInForce { get; set; }

        [Display(Name = "Trail Amt")]
        public double? TrailingAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? UsdQuantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N1} pips")]
        public double? Slippage { get; set; }

        [Display(Name = "Virtual")]
        public bool IsVirtual { get; set; }

        public override string ToString()
        {
            return $"{Side} {Quantity}K {Cross}";
        }
    }
}
