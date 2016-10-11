using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.AspNetCore.Models;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    public class ExecutionsController : Controller
    {
        private readonly ExecutionsControllerUtils utils;

        public ExecutionsController(MonitorDbContext db)
        {
            utils = new ExecutionsControllerUtils(db);
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(day));
        }

        public async Task<IActionResult> Details(string executionid)
        {
            return View(await utils.GetById(executionid));
        }
    }
}
