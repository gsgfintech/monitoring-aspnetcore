using Capital.GSG.FX.Data.Core.ExecutionData;
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
    public class ExecutionsControllerUtils
    {
        private readonly MonitorDbContext db;

        public ExecutionsControllerUtils(MonitorDbContext db)
        {
            this.db = db;
        }

        internal async Task<ExecutionsListViewModel> CreateListViewModel(DateTime? day = null)
        {
            if (!day.HasValue)
            {
                if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                    day = DateTime.Today.AddDays(-1);
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                    day = DateTime.Today.AddDays(-2);
                else
                    day = DateTime.Today;
            }

            List<ExecutionModel> trades = await GetExecutionsForDay(day.Value);

            return new ExecutionsListViewModel(day.Value, trades ?? new List<ExecutionModel>());
        }

        private async Task<List<ExecutionModel>> GetExecutionsForDay(DateTime day)
        {
            Tuple<DateTimeOffset, DateTimeOffset> boundaries = DateTimeUtils.GetTradingDayBoundariesDateTimeOffset(day);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            return (await (from e in db.Executions
                           where e.ExecutionTime >= boundaries.Item1
                           where e.ExecutionTime <= boundaries.Item2
                           orderby e.ExecutionTime descending
                           select e).ToListAsync()).ToExecutionModels();
        }

        internal async Task<ExecutionModel> GetById(string id)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            // TODO : replace Where().FirstOrDefaultAsync() by FindAsync once the method is implemented in EF Core
            return (await db.Executions.Where(e => e.Id == id).FirstOrDefaultAsync(cts.Token)).ToExecutionModel();
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
