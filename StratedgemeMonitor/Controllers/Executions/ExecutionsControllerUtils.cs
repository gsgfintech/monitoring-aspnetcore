using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.ExecutionData;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using StratedgemeMonitor.Models.Executions;
using StratedgemeMonitor.ViewModels.Executions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Executions
{
    public class ExecutionsControllerUtils
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<ExecutionsControllerUtils>();

        private readonly BackendExecutionsConnector connector;
        private readonly Broker broker = Broker.IB; // TODO

        private DateTime currentDay = DateTimeUtils.GetLastBusinessDayInHKT();

        public ExecutionsControllerUtils(BackendExecutionsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<ExecutionsListViewModel> CreateListViewModel(DateTime? day = null)
        {
            if (day.HasValue)
                currentDay = day.Value;

            List<ExecutionModel> trades = await GetExecutions();

            return new ExecutionsListViewModel(currentDay, trades ?? new List<ExecutionModel>());
        }

        private async Task<List<ExecutionModel>> GetExecutions()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var result = await connector.GetForDay(broker, currentDay);

            if (!result.Success)
                logger.Error(result.Message);

            return result.Trades?.AsEnumerable().OrderByDescending(e => e.ExecutionTime).ToExecutionModels();
        }

        internal async Task<ExecutionModel> GetById(string id)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var result = await connector.GetById(broker, id);

            if (!result.Success)
                logger.Error(result.Message);

            return result.Trade.ToExecutionModel();
        }

        internal async Task<int> GetTodaysTradesCount()
        {
            return (await connector.GetForDay(broker, DateTime.Today)).Trades?.Count ?? 0;
        }

        internal async Task<FileResult> ExportExcel()
        {
            string fileName = $"Trades-{currentDay:yyyy-MM-dd}.xlsx";
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
            List<ExecutionModel> executions = await GetExecutions();

            if (executions.IsNullOrEmpty())
                return new byte[0];

            string[] headers = new string[20]
            {
                "Execution Time",
                "Origin",
                "Side",
                "Quantity",
                "Cross",
                "Rate",
                "Position",
                "PnL",
                "PnL Ccy",
                "PnL USD",
                "Commission",
                "Commission Ccy",
                "Commission USD",
                "Strategy",
                "Execution ID",
                "Order ID",
                "Order Permanent ID",
                "Exchange",
                "Account",
                "Our Ref"
            };

            using (ExcelPackage excel = new ExcelPackage())
            {
                var groups = executions.GroupBy(e => e.Cross).OrderBy(g => g.Key);

                foreach (var group in groups)
                {
                    List<object[]> dataset = new List<object[]>();

                    Cross cross = group.Key;

                    double cumulativePosition = 0;

                    string pnlCcy = "";
                    string commissionCcy = "";

                    foreach (var execution in group)
                    {
                        try
                        {
                            // Update cumulative position
                            cumulativePosition += execution.Side == ExecutionSide.BOUGHT ? execution.Quantity : -1 * execution.Quantity;

                            object[] row = new object[20];

                            row[0] = execution.ExecutionTime.ToLocalTime();
                            row[1] = execution.OrderOrigin.ToString();
                            row[2] = execution.Side.ToString();
                            row[3] = execution.Quantity;
                            row[4] = execution.Cross.ToString();
                            row[5] = execution.Price;
                            row[6] = cumulativePosition;
                            row[7] = execution.RealizedPnL;
                            row[8] = CrossUtils.GetQuotedCurrency(execution.Cross).ToString();
                            row[9] = execution.RealizedPnlUsd;
                            row[10] = execution.Commission;
                            row[11] = execution.CommissionCcy.ToString();
                            row[12] = execution.CommissionUsd.ToString();
                            row[13] = execution.Strategy;
                            row[14] = execution.Id;
                            row[15] = execution.OrderId;
                            row[16] = execution.PermanentID;
                            row[17] = execution.Exchange;
                            row[18] = execution.AccountNumber;
                            row[19] = execution.ClientOrderRef;

                            // Keep for later
                            pnlCcy = (string)row[8];
                            commissionCcy = (string)row[11];

                            dataset.Add(row);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Failed to add execution to the dataset", ex);
                        }
                    }

                    // Create the worksheet
                    ExcelWorksheet ws = excel.Workbook.Worksheets.Add($"{cross}-{currentDay:yyyyMMdd}");

                    ws.Cells["A1"].LoadFromArrays(new string[1][] { headers });
                    ws.Cells["A2"].LoadFromArrays(dataset);

                    // Format numbers
                    #region Timestamp
                    using (ExcelRange col = ws.Cells[2, 1, 2 + dataset.Count(), 1])
                    {
                        col.Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";
                    }
                    #endregion

                    #region Quantity
                    using (ExcelRange col = ws.Cells[2, 4, 2 + dataset.Count(), 4])
                    {
                        col.Style.Numberformat.Format = "#,##0";
                    }
                    #endregion

                    #region Rate
                    using (ExcelRange col = ws.Cells[2, 6, 2 + dataset.Count(), 6])
                    {
                        col.Style.Numberformat.Format = (pnlCcy == "JPY") ? "0.00" : "0.00000";
                    }
                    #endregion

                    #region Position
                    using (ExcelRange col = ws.Cells[2, 7, 2 + dataset.Count(), 7])
                    {
                        col.Style.Numberformat.Format = "#,##0";
                        col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    #endregion

                    #region PnL
                    using (ExcelRange col = ws.Cells[2, 8, 2 + dataset.Count(), 8])
                    {
                        col.Style.Numberformat.Format = "#,##0.00";
                    }

                    using (ExcelRange col = ws.Cells[2, 10, 2 + dataset.Count(), 10])
                    {
                        col.Style.Numberformat.Format = "#,##0.00";
                    }
                    #endregion

                    #region Commission
                    using (ExcelRange col = ws.Cells[2, 11, 2 + dataset.Count(), 11])
                    {
                        col.Style.Numberformat.Format = "#,##0.00";
                    }

                    using (ExcelRange col = ws.Cells[2, 13, 2 + dataset.Count(), 13])
                    {
                        col.Style.Numberformat.Format = "#,##0.00";
                    }
                    #endregion

                    // Format as table
                    var tableRng = ws.Cells[1, 1, dataset.Count() + 1, headers.Length];
                    var table = ws.Tables.Add(tableRng, $"Trades-{cross}");
                    table.TableStyle = TableStyles.Light9;
                    table.ShowHeader = true;
                    table.ShowTotal = true;
                    table.Columns["Position"].TotalsRowLabel = cumulativePosition.ToString();
                    table.Columns["PnL"].TotalsRowFormula = "SUBTOTAL(109,[PnL])";
                    table.Columns["PnL Ccy"].TotalsRowLabel = pnlCcy;
                    table.Columns["PnL USD"].TotalsRowFormula = "SUBTOTAL(109,[PnL USD])";
                    table.Columns["Commission"].TotalsRowFormula = "SUBTOTAL(109,[Commission])";
                    table.Columns["Commission Ccy"].TotalsRowLabel = commissionCcy;
                    table.Columns["Commission USD"].TotalsRowFormula = "SUBTOTAL(109,[Commission USD])";

                    // Finally autofit all columns
                    ws.Cells[1, 1, dataset.Count() + 1, headers.Length].AutoFitColumns();
                }

                return excel.GetAsByteArray();
            }
        }
    }

    public static class ExecutionsControllerUtilsExtensions
    {
        public static ExecutionModel ToExecutionModel(this Execution execution)
        {
            if (execution == null)
                return null;

            return new ExecutionModel()
            {
                AccountNumber = execution.AccountNumber,
                ClientId = execution.ClientId,
                ClientOrderRef = execution.ClientOrderRef,
                Commission = execution.Commission,
                CommissionCcy = execution.CommissionCcy,
                CommissionUsd = execution.CommissionUsd,
                Cross = execution.Cross,
                Exchange = execution.Exchange,
                ExecutionTime = execution.ExecutionTime.ToOffset(TimeSpan.FromHours(8)),
                Id = execution.Id,
                OrderId = execution.OrderId,
                OrderOrigin = execution.OrderOrigin,
                PermanentID = execution.PermanentID,
                Price = execution.Price,
                Quantity = execution.Quantity / 1000,
                RealizedPnL = execution.RealizedPnL,
                RealizedPnlPips = execution.RealizedPnlPips,
                RealizedPnlUsd = execution.RealizedPnlUsd,
                Side = execution.Side,
                Strategy = execution.Strategy,
                TradeDuration = execution.TradeDuration
            };
        }

        public static List<ExecutionModel> ToExecutionModels(this IEnumerable<Execution> executions)
        {
            return executions?.Select(e => e.ToExecutionModel()).ToList();
        }
    }
}
