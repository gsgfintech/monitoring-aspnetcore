using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.IB.FutureContracts
{
    public class FutureContractsController
    {
        private readonly FutureContractsControllerUtils utils;

        public FutureContractsController(FutureContractsControllerUtils utils)
        {
            this.utils = utils;
        }
    }
}
