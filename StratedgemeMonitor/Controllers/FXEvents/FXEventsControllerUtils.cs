using Capital.GSG.FX.Data.Core.MarketData;
using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.Models.FXEvents;
using StratedgemeMonitor.ViewModels.FXEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.FXEvents
{
    public class FXEventsControllerUtils
    {
        private readonly BackendFXEventsConnector connector;

        public FXEventsControllerUtils(BackendFXEventsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<FXEventsListViewModel> CreateListViewModel()
        {
            DateTimeOffset start = DateTime.Today.AddDays(-1 * (int)DateTime.Today.DayOfWeek);
            DateTimeOffset end = DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek - 7));

            List<FXEventModel> currentWeeksFXEvents = (await GetFXEventsInTimeRange(start, end)) ?? new List<FXEventModel>();
            List<FXEventModel> todaysHighImpactFXEvents = (await GetHighImpactForToday()) ?? new List<FXEventModel>();

            return new FXEventsListViewModel()
            {
                CurrentWeekEnd = end,
                CurrentWeeksFXEvents = currentWeeksFXEvents,
                CurrentWeekStart = start,
                TodaysHighImpactFXEvents = todaysHighImpactFXEvents
            };
        }

        private async Task<List<FXEventModel>> GetFXEventsInTimeRange(DateTimeOffset start, DateTimeOffset end)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var fxEvents = await connector.GetInTimeRange(start, end, cts.Token);

            return fxEvents?.AsEnumerable().OrderBy(e => e.Timestamp).ToFXEventModels();
        }

        private async Task<List<FXEventModel>> GetHighImpactForToday()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var fxEvents = await connector.GetHighImpactForToday(cts.Token);

            return fxEvents?.AsEnumerable().OrderBy(e => e.Timestamp).ToFXEventModels();
        }

        internal async Task<FXEventModel> GetById(string id)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var fxEvent = await connector.GetById(id, cts.Token);

            return fxEvent.ToFXEventModel();
        }
    }

    public static class FXEventsControllerUtilsExtensions
    {
        public static FXEventModel ToFXEventModel(this FXEvent fxEvent)
        {
            if (fxEvent == null)
                return null;

            return new FXEventModel()
            {
                Actual = fxEvent.Actual,
                Currency = fxEvent.Currency,
                Explanation = fxEvent.Explanation,
                Forecast = fxEvent.Forecast,
                Id = fxEvent.EventId,
                Level = fxEvent.Level,
                Previous = fxEvent.Previous,
                Timestamp = fxEvent.Timestamp.ToOffset(TimeSpan.FromHours(8)),
                Title = fxEvent.Title
            };
        }

        public static List<FXEventModel> ToFXEventModels(this IEnumerable<FXEvent> fxEvents)
        {
            return fxEvents?.Select(e => e.ToFXEventModel()).ToList();
        }
    }
}
