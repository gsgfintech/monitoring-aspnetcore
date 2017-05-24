using StratedgemeMonitor.Models.IB.FutureContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.ViewModels.IB.FutureContracts
{
    public class FutureContractsListViewModel
    {
        public List<FutureContractModel> Contracts { get; private set; }
        public string AppEndpoint { get; private set; }

        public FutureContractsListViewModel(List<FutureContractModel> contracts, string appEndpoint)
        {
            Contracts = contracts;
            AppEndpoint = appEndpoint;
        }
    }
}
