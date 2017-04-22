using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.ViewModels.FAConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.FAConfigurations
{
    [Authorize]
    public class FAConfigurationsController : Controller
    {
        private readonly FAConfigurationsControllerUtils utils;

        public FAConfigurationsController(FAConfigurationsControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index()
        {
            var configurations = await utils.GetAll();

            return View(new FAConfigurationsIndexViewModel(configurations));
        }

        public async Task<IActionResult> Details(string masterAccount)
        {
            var configuration = await utils.Get(masterAccount);

            return View(configuration);
        }
    }
}
