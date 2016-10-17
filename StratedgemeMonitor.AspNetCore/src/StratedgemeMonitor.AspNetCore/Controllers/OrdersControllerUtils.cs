using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.OrderData;
using Capital.GSG.FX.MongoConnector.Core;
using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    internal class OrdersControllerUtils
    {
        private readonly MongoDBServer mongoDBServer;

        private readonly OrderStatusCode[] activeStatus = new OrderStatusCode[3] { OrderStatusCode.PendingSubmit, OrderStatusCode.PreSubmitted, OrderStatusCode.Submitted };

        public OrdersControllerUtils(MongoDBServer mongoDBServer)
        {
            this.mongoDBServer = mongoDBServer;
        }

        internal async Task<OrdersListViewModel> CreateListViewModel(DateTime? day = null)
        {
            if (!day.HasValue)
                day = DateTimeUtils.GetLastBusinessDayInHKT();

            List<OrderModel> activeOrders = await GetActiveOrders();
            List<OrderModel> inactiveOrders = await GetInactiveOrdersForDay(day.Value);

            return new OrdersListViewModel(day.Value, activeOrders, inactiveOrders);
        }

        private async Task<List<OrderModel>> GetInactiveOrdersForDay(DateTime day)
        {
            return (await GetOrdersForDay(day))?.Where(o => !activeStatus.Contains(o.Status))?.ToList();
        }

        internal async Task<List<OrderModel>> GetOrdersForDay(DateTime day)
        {
            Tuple<DateTimeOffset, DateTimeOffset> boundaries = DateTimeUtils.GetTradingDayBoundariesDateTimeOffset(day);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            return (await mongoDBServer.OrderActioner.GetOrdersPlacedOnDay(day, cts.Token)).ToOrderModels();
        }

        private async Task<List<OrderModel>> GetActiveOrders()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            return (await mongoDBServer.OrderActioner.GetByStatus(activeStatus, ct: cts.Token)).ToOrderModels();
        }

        internal async Task<OrderModel> GetByPermanentId(int permanentId)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            return (await mongoDBServer.OrderActioner.Get(permanentId, cts.Token)).ToOrderModel();
        }

        internal async Task<bool> AddOrUpdate(Order order)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            return await mongoDBServer.OrderActioner.AddOrUpdate(order, cts.Token);
        }
    }

    internal static class OrdersControllerUtilsExtensions
    {
        public static List<OrderModel> ToOrderModels(this IEnumerable<Order> orders)
        {
            return orders?.Select(o => o.ToOrderModel()).ToList();
        }

        private static OrderHistoryPointModel ToOrderHistoryPointModel(this OrderHistoryPoint point)
        {
            if (point == null)
                return null;

            return new OrderHistoryPointModel()
            {
                Status = point.Status,
                Timestamp = point.Timestamp.ToOffset(TimeSpan.FromHours(8))
            };
        }

        private static List<OrderHistoryPointModel> ToOrderHistoryPointModels(this IEnumerable<OrderHistoryPoint> points)
        {
            return points?.Select(p => p.ToOrderHistoryPointModel()).ToList();
        }

        public static OrderModel ToOrderModel(this Order order)
        {
            if (order == null)
                return null;

            return new OrderModel()
            {
                ClientId = order.ClientID,
                Cross = order.Cross,
                EstimatedCommission = order.EstimatedCommission,
                EstimatedCommissionCcy = order.EstimatedCommissionCcy,
                FillPrice = order.FillPrice,
                History = order.History.ToOrderHistoryPointModels(),
                LastAsk = order.LastAsk,
                LastBid = order.LastBid,
                LastMid = order.LastMid,
                LastUpdateTime = order.LastUpdateTime?.ToOffset(TimeSpan.FromHours(8)),
                LimitPrice = order.LimitPrice,
                OrderId = order.OrderID,
                Origin = order.Origin,
                OurRef = order.OurRef,
                ParentOrderId = order.ParentOrderID,
                PermanentId = order.PermanentID,
                PlacedTime = order.PlacedTime?.ToOffset(TimeSpan.FromHours(8)),
                Quantity = order.Quantity / 1000,
                Side = order.Side,
                Slippage = CalculateSlippage(order),
                Status = order.Status,
                StopPrice = order.StopPrice,
                Strategy = order.Strategy,
                TimeInForce = order.TimeInForce,
                TrailingAmount = order.TrailingAmount,
                Type = order.Type,
                UsdQuantity = order.UsdQuantity
            };
        }

        private static double? CalculateSlippage(Order order)
        {
            if (order.Side != OrderSide.UNKNOWN && order.FillPrice.HasValue)
            {
                if (order.Side == OrderSide.BUY && order.LastAsk.HasValue) // we buy the ask
                    return CrossUtils.ConvertToFractionalPips(order.FillPrice.Value - order.LastAsk.Value, order.Cross);
                else if (order.Side == OrderSide.SELL && order.LastBid.HasValue) // we sell the bid
                    return CrossUtils.ConvertToFractionalPips(order.LastBid.Value - order.FillPrice.Value, order.Cross);
            }

            return null;
        }
    }
}
