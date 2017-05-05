using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.Models.Executions;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.ViewModels.Executions
{
    public class ExecutionsListViewModel
    {
        public List<ExecutionModel> Trades { get; private set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTime Day { get; private set; }

        public List<Series> PnlChartData { get; private set; }
        public Axis PnlChartXAxis { get; private set; }
        public Axis PnlChartYAxis { get; private set; }

        public string GrafanaEndpoint { get; private set; }
        public long GraphStartTimeUnixTicks { get; private set; }

        public ExecutionsListViewModel(DateTime day, List<ExecutionModel> trades, string grafanaEndpoint)
        {
            Day = day;
            Trades = trades;

            //CreatePnlChart(day, trades);

            GrafanaEndpoint = grafanaEndpoint;
            GraphStartTimeUnixTicks = DateTimeUtils.GetFivePmYesterday().ToUnixTimeMilliseconds();
        }

        private void CreatePnlChart(DateTime day, List<ExecutionModel> trades)
        {
            if (!trades.IsNullOrEmpty())
            {
                Tuple<DateTime, DateTime> tradingDayBoundaries = DateTimeUtils.GetTradingDayBoundaries(day);

                PnlChartData = new List<Series>();

                List<Tuple<DateTime, double>> cumulativePnl = new List<Tuple<DateTime, double>>();

                double curPnl = 0;
                foreach (var trade in trades.AsEnumerable().Reverse())
                {
                    if (trade.RealizedPnlUsd.HasValue)
                    {
                        curPnl += trade.RealizedPnlUsd.Value;
                        cumulativePnl.Add(new Tuple<DateTime, double>(trade.ExecutionTime.LocalDateTime, curPnl));
                    }
                }

                PnlChartData.Add(new Series()
                {
                    DataSource = cumulativePnl,
                    Marker = new Marker()
                    {
                        Shape = ChartShape.Circle,
                        Visible = true
                    },
                    Name = "Cumulative PnL (USD)",
                    Tooltip = new NewTooltip()
                    {
                        Visible = true
                    },
                    Type = SeriesType.Line,
                    XName = "Item1",
                    XAxisName = "Time (HKT)",
                    YName = "Item2",
                    YAxisName = "Cumulative PnL (USD)"
                });

                PnlChartXAxis = new Axis()
                {
                    IntervalType = ChartIntervalType.Hours,
                    LabelFormat = "HH:mm",
                    Range = new Range()
                    {
                        Interval = 1,
                        Max = tradingDayBoundaries.Item2,
                        Min = tradingDayBoundaries.Item1
                    },
                    ValueType = AxisValueType.Datetime
                };

                int yUpperBound = !cumulativePnl.IsNullOrEmpty() ? ((int)Math.Max(0, cumulativePnl.Select(i => i.Item2).Max())) + 1 : 100;
                int yLowerBound = !cumulativePnl.IsNullOrEmpty() ? ((int)Math.Min(0, cumulativePnl.Select(i => i.Item2).Min())) - 1 : -100;

                PnlChartYAxis = new Axis()
                {
                    LabelFormat = "c0",
                    Range = new Range()
                    {
                        Interval = Math.Max(1, (yUpperBound - yLowerBound) / 10),
                        Max = yUpperBound,
                        Min = yLowerBound
                    },
                    ValueType = AxisValueType.Double
                };
            }
        }
    }
}
