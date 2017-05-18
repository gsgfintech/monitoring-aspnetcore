using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.Models.Stratedgeme.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Stratedgeme.Strategies
{
    [Authorize]
    public class StrategiesController : Controller
    {
        private readonly StrategiesControllerUtils utils;

        public StrategiesController(StrategiesControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index()
        {
            return View(await utils.Index());
        }

        public async Task<IActionResult> Available()
        {
            return View(await utils.Available());
        }

        public async Task<IActionResult> Details(string name, string version)
        {
            return View(await utils.Details(name, version));
        }

        public async Task<IActionResult> Edit(string name, string version)
        {
            return View(await utils.Edit(name, version));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoEdit(StrategyModel strategy)
        {
            return View("Details", await utils.DoEdit(strategy));
        }
    }
}
