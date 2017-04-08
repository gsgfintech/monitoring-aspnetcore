using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Executions
{
    [Authorize]
    public class ExecutionsController : Controller
    {
        private readonly ExecutionsControllerUtils utils;

        public ExecutionsController(ExecutionsControllerUtils utils)
        {
            this.utils = utils;
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

        public async Task<FileResult> ExportExcel()
        {
            return await utils.ExportExcel();
        }
    }
}
