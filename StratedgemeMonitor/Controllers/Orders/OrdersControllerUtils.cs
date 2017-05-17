using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.FinancialAdvisorsData;
using Capital.GSG.FX.Data.Core.OrderData;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using StratedgemeMonitor.Models.Orders;
using StratedgemeMonitor.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Orders
{
    public class OrdersControllerUtils
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<OrdersControllerUtils>();

        private readonly BackendOrdersConnector connector;
        private readonly Broker broker = Broker.IB; // TODO

        private readonly OrderStatusCode[] activeStatus = new OrderStatusCode[3] { OrderStatusCode.PendingSubmit, OrderStatusCode.PreSubmitted, OrderStatusCode.Submitted };

        private DateTime currentDay = DateTimeUtils.GetLastBusinessDayInHKT();

        public OrdersControllerUtils(BackendOrdersConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<OrdersListViewModel> CreateListViewModel(DateTime? day = null)
        {
            if (day.HasValue)
                currentDay = day.Value;

            List<OrderModel> activeOrders = await GetActiveOrders();
            List<OrderModel> inactiveOrders = await GetInactiveOrdersForDay();

            return new OrdersListViewModel(currentDay, activeOrders, inactiveOrders);
        }

        private async Task<List<OrderModel>> GetInactiveOrdersForDay()
        {
            return (await GetOrders())?.Where(o => !activeStatus.Contains(o.Status))?.ToList();
        }

        internal async Task<List<OrderModel>> GetOrders()
        {
            var result = await connector.GetOrdersForDay(currentDay);

            if (!result.Success)
                logger.Error(result.Message);

            return result.Orders?.AsEnumerable().OrderByDescending(o => o.PlacedTime).ToOrderModels();
        }

        private async Task<List<OrderModel>> GetActiveOrders()
        {
            var result = await connector.GetActiveOrders();

            if (!result.Success)
                logger.Error(result.Message);

            return result.Orders?.AsEnumerable().OrderByDescending(o => o.PlacedTime).ToOrderModels();
        }

        internal async Task<int> GetActiveOrdersCount()
        {
            var result = await connector.GetActiveOrders();

            if (!result.Success)
                logger.Error(result.Message);

            return result.Orders?.Count ?? 0;
        }

        internal async Task<int> GetInactiveOrdersCount()
        {
            var result = await connector.GetOrdersForDay(DateTime.Today);

            if (!result.Success)
                logger.Error(result.Message);

            return result.Orders?.Count ?? 0;
        }

        internal async Task<OrderModel> GetByPermanentId(long permanentId)
        {
            var result = await connector.Get(broker, permanentId);

            if (!result.Success)
                logger.Error(result.Message);

            return result.Order?.ToOrderModel();
        }

        internal async Task<FileResult> ExportExcel()
        {
            string fileName = $"Orders-{currentDay:yyyy-MM-dd}.xlsx";
            string contentType = "Application/msexcel";

            byte[] bytes = await CreateExcel();

            MemoryStream stream = new MemoryStream(bytes);

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = fileName
            };
        }

        private async Task<byte[]> CreateExcel()
        {
            var orders = await GetOrders();

            if (orders.IsNullOrEmpty())
                return new byte[0];

            List<Dictionary<string, object>> dataset = new List<Dictionary<string, object>>();

            foreach (var order in orders)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();

                foreach (var property in order.GetType().GetProperties())
                {
                    data.Add(property.Name, property.GetValue(order));
                }

                dataset.Add(data);
            }

            // Cleanup
            foreach (var data in dataset)
            {
                data.Remove("IBClientID");
                data.Remove("Contract");
                data.Remove("TransmitOrder");
                data.Remove("Warning");

                if (data.ContainsKey("PlacedTime") && data["PlacedTime"] is DateTime)
                    data["PlacedTime"] = ((DateTime)data["PlacedTime"]).ToLocalTime();
            }

            IEnumerable<object[]> values = dataset.Select(d => d.Values.ToArray());

            string[] headers = dataset.First().Keys.ToArray();

            using (ExcelPackage excel = new ExcelPackage())
            {
                // Create the worksheet
                ExcelWorksheet ws = excel.Workbook.Worksheets.Add($"Orders {currentDay:yyyyMMdd}");

                ws.Cells["A1"].LoadFromArrays(new string[1][] { headers });
                ws.Cells["A2"].LoadFromArrays(values);

                // Freeze top row and first column
                ws.View.FreezePanes(2, 2);

                // Make headers bold
                using (ExcelRange row = ws.Cells[1, 1, 1, headers.Length])
                {
                    row.Style.Font.Bold = true;
                }

                // Change the number format of the Timestamp column
                using (ExcelRange col = ws.Cells[2, 5, 2 + values.Count(), 5])
                {
                    col.Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";
                }

                using (ExcelRange col = ws.Cells[2, 6, 2 + values.Count(), 6])
                {
                    col.Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";
                }

                // Finally autofit all columns
                //ws.Cells[1, 1, dataset.Count() + 1, headers.Length].AutoFitColumns();

                return excel.GetAsByteArray();
            }
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
                Message = point.Message,
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
                Account = order.Account,
                AllocationInfo = ComputeAllocationInfo(order),
                Broker = order.Broker,
                Cross = order.Cross,
                EstimatedCommission = order.EstimatedCommission,
                EstimatedCommissionCcy = order.EstimatedCommissionCcy,
                FillPrice = order.FillPrice,
                History = order.History.ToOrderHistoryPointModels(),
                IsVirtual = order.IsVirtual,
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

        private static object ComputeAllocationInfo(Order order)
        {
            if (string.IsNullOrEmpty(order.AllocationInfo))
                return "No allocation information";

            // 1. Try parse allocation profile
            FAAllocationProfile allocationProfile = null;
            try { allocationProfile = JsonConvert.DeserializeObject<FAAllocationProfile>(order.AllocationInfo); } catch { }

            if (allocationProfile != null)
                return allocationProfile;

            // 2. Try parse FA Group
            FAGroup group = null;
            try { group = JsonConvert.DeserializeObject<FAGroup>(order.AllocationInfo); } catch { }

            if (group != null)
                return group;

            return order.AllocationInfo;
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
