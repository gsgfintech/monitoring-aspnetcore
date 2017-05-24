using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.Models.IB.FutureContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.IB.FutureContracts
{
    public class FutureContractsControllerUtils
    {
        private readonly BackendIBFutureContractsConnector connector;

        public FutureContractsControllerUtils(BackendIBFutureContractsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<(bool Success, string Message, List<FutureContractModel> Contracts)> GetAll()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var result = await connector.GetAll(cts.Token);

            return (result.Success, result.Message, result.Contracts.ToFutureContractModels());
        }

        internal async Task<(bool Success, string Message)> AddOrUpdate(FutureContractModel contract)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            return await connector.AddOrUpdate(contract.ToFutureContract(), cts.Token);
        }

        internal async Task<(bool Success, string Message)> Delete(string exchange, string symbol, double multiplier)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            return await connector.Delete(exchange, symbol, multiplier, cts.Token);
        }
    }
}
