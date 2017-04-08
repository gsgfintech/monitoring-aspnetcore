using Capital.GSG.FX.Data.Core.OrderData;
using System;

namespace StratedgemeMonitor.Models.Orders
{
    public class OrderHistoryPointModel
    {
        public OrderStatusCode Status { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
