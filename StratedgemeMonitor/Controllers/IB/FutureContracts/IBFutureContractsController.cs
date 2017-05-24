using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.IB.FutureContracts
{
    [Authorize]
    public class IBFutureContractsController : Controller
    {
        private readonly IBFutureContractsControllerUtils utils;

        public IBFutureContractsController(IBFutureContractsControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.Index());
        }
    }
}
