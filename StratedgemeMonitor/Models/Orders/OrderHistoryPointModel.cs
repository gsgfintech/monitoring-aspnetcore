using Capital.GSG.FX.Data.Core.OrderData;
using System;

namespace StratedgemeMonitor.Models.Orders
{
    public class OrderHistoryPointModel
    {
        public string Message { get; set; }
        public OrderStatusCode Status { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
