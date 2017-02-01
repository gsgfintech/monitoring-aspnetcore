using Capital.GSG.FX.Data.Core.MarketData;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Microsoft.AspNetCore.Http;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.Utils;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    internal class FXEventsControllerUtils
    {
        private readonly BackendFXEventsConnector connector;

        public FXEventsControllerUtils(BackendFXEventsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<FXEventsListViewModel> CreateListViewModel(ISession session, ClaimsPrincipal user)
        {
            DateTimeOffset start = DateTime.Today.AddDays(-1 * (int)DateTime.Today.DayOfWeek);
            DateTimeOffset end = DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek - 7));

            List<FXEventModel> currentWeeksFXEvents = (await GetFXEventsInTimeRange(start, end, session, user)) ?? new List<FXEventModel>();
            List<FXEventModel> todaysHighImpactFXEvents = (await GetHighImpactForToday(session, user)) ?? new List<FXEventModel>();

            return new FXEventsListViewModel()
            {
                CurrentWeekEnd = end,
                CurrentWeeksFXEvents = currentWeeksFXEvents,
                CurrentWeekStart = start,
                TodaysHighImpactFXEvents = todaysHighImpactFXEvents
            };
        }

        private async Task<List<FXEventModel>> GetFXEventsInTimeRange(DateTimeOffset start, DateTimeOffset end, ISession session, ClaimsPrincipal user)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var fxEvents = await connector.GetInTimeRange(start, end, cts.Token);

            return fxEvents?.AsEnumerable().OrderBy(e => e.Timestamp).ToFXEventModels();
        }

        private async Task<List<FXEventModel>> GetHighImpactForToday(ISession session, ClaimsPrincipal user)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var fxEvents = await connector.GetHighImpactForToday(cts.Token);

            return fxEvents?.AsEnumerable().OrderBy(e => e.Timestamp).ToFXEventModels();
        }

        internal async Task<FXEventModel> GetById(string id, ISession session, ClaimsPrincipal user)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var fxEvent = await connector.GetById(id, cts.Token);

            return fxEvent.ToFXEventModel();
        }
    }

    internal static class FXEventsControllerUtilsExtensions
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
