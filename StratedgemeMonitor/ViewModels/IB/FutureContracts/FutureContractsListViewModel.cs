using StratedgemeMonitor.Models.IB.FutureContracts;
using System.Collections.Generic;

namespace StratedgemeMonitor.ViewModels.IB.FutureContracts
{
    public class FutureContractsListViewModel
    {
        public List<FutureContractModel> Contracts { get; set; }
        public string AppEndpoint { get; set; }
        public string Error { get; set; }
    }
}
