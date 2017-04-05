using StratedgemeMonitor.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.ViewModels.Orders
{
    public class OrdersListViewModel
    {
        public List<OrderModel> ActiveOrders { get; set; }
        public List<OrderModel> InactiveOrders { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTime Day { get; private set; }

        public OrdersListViewModel(DateTime day, List<OrderModel> activeOrders, List<OrderModel> inactiveOrders)
        {
            Day = day;

            ActiveOrders = activeOrders ?? new List<OrderModel>();
            InactiveOrders = inactiveOrders ?? new List<OrderModel>();
        }
    }
}
