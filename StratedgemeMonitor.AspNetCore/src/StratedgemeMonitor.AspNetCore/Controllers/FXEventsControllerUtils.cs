using Capital.GSG.FX.Data.Core.MarketData;
using Capital.GSG.FX.Utils.Core;
using Microsoft.EntityFrameworkCore;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    internal class FXEventsControllerUtils
    {
        private readonly MonitorDbContext db;

        public FXEventsControllerUtils(MonitorDbContext db)
        {
            this.db = db;
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

            return (await (from e in db.FXEvents
                           where e.Timestamp >= start
                           where e.Timestamp <= end
                           orderby e.Timestamp
                           select e).ToListAsync(cts.Token)).ToFXEventModels();
        }

        private async Task<List<FXEventModel>> GetHighImpactForToday()
        {
            Tuple<DateTimeOffset, DateTimeOffset> boundaries = DateTimeUtils.GetTradingDayBoundariesDateTimeOffset(DateTime.Today);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            return (await (from e in db.FXEvents
                           where e.Timestamp >= boundaries.Item1
                           where e.Timestamp <= boundaries.Item2
                           where e.Level == FXEventLevel.HIGH
                           orderby e.Timestamp
                           select e).ToListAsync(cts.Token)).ToFXEventModels();
        }

        internal async Task<FXEventModel> GetById(string id)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            // TODO : replace FirstOrDefaultAsync() by FindAsync once the method is implemented in EF Core
            return (await db.FXEvents.FirstOrDefaultAsync(e => e.EventId == id, cts.Token)).ToFXEventModel();
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
