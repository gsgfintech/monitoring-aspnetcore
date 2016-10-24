using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.OrderData;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core;
using Microsoft.AspNetCore.Http;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.Utils;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    internal class OrdersControllerUtils
    {
        private readonly BackendOrdersConnector connector;

        private readonly OrderStatusCode[] activeStatus = new OrderStatusCode[3] { OrderStatusCode.PendingSubmit, OrderStatusCode.PreSubmitted, OrderStatusCode.Submitted };

        public OrdersControllerUtils(BackendOrdersConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<OrdersListViewModel> CreateListViewModel(ISession session, ClaimsPrincipal user, DateTime? day = null)
        {
            if (!day.HasValue)
                day = DateTimeUtils.GetLastBusinessDayInHKT();

            List<OrderModel> activeOrders = await GetActiveOrders(session, user);
            List<OrderModel> inactiveOrders = await GetInactiveOrdersForDay(day.Value, session, user);

            return new OrdersListViewModel(day.Value, activeOrders, inactiveOrders);
        }

        private async Task<List<OrderModel>> GetInactiveOrdersForDay(DateTime day, ISession session, ClaimsPrincipal user)
        {
            return (await GetOrdersForDay(day, session, user))?.Where(o => !activeStatus.Contains(o.Status))?.ToList();
        }

        internal async Task<List<OrderModel>> GetOrdersForDay(DateTime day, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            var orders = await connector.GetOrdersForDay(day, accessToken);

            return orders?.AsEnumerable().OrderByDescending(o => o.PlacedTime).ToOrderModels();
        }

        private async Task<List<OrderModel>> GetActiveOrders(ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return (await connector.GetActiveOrders(accessToken))?.AsEnumerable().OrderByDescending(o => o.PlacedTime).ToOrderModels();
        }

        internal async Task<OrderModel> GetByPermanentId(int permanentId, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return (await connector.GetOrderByPermanentId(permanentId, accessToken)).ToOrderModel();
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
