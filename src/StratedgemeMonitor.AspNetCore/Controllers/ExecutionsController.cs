using Capital.GSG.FX.Monitoring.Server.Connector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class ExecutionsController : Controller
    {
        private readonly ExecutionsControllerUtils utils;

        public ExecutionsController(BackendExecutionsConnector connector)
        {
            utils = new ExecutionsControllerUtils(connector);
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User, day));
        }

        public async Task<IActionResult> Details(string executionid)
        {
            return View(await utils.GetById(executionid, HttpContext.Session, User));
        }
    }
}
