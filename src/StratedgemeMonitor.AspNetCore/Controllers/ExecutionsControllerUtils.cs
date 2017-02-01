using Capital.GSG.FX.Data.Core.ExecutionData;
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
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    public class ExecutionsControllerUtils
    {
        private readonly BackendExecutionsConnector connector;

        public ExecutionsControllerUtils(BackendExecutionsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<ExecutionsListViewModel> CreateListViewModel(ISession session, ClaimsPrincipal user, DateTime? day = null)
        {
            if (!day.HasValue)
                day = DateTimeUtils.GetLastBusinessDayInHKT();

            List<ExecutionModel> trades = await GetExecutionsForDay(day.Value, session, user);

            return new ExecutionsListViewModel(day.Value, trades ?? new List<ExecutionModel>());
        }

        private async Task<List<ExecutionModel>> GetExecutionsForDay(DateTime day, ISession session, ClaimsPrincipal user)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var executions = await connector.GetForDay(day);

            return executions?.AsEnumerable().OrderByDescending(e => e.ExecutionTime).ToExecutionModels();
        }

        internal async Task<ExecutionModel> GetById(string id, ISession session, ClaimsPrincipal user)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var execution = await connector.GetById(id);

            return execution.ToExecutionModel();
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
