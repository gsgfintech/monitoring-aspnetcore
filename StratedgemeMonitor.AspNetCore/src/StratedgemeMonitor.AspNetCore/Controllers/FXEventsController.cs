using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.AspNetCore.Models;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class FXEventsController : Controller
    {
        private readonly FXEventsControllerUtils utils;

        public FXEventsController(MonitorDbContext db)
        {
            utils = new FXEventsControllerUtils(db);
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel());
        }

        public async Task<IActionResult> Details(string id)
        {
            return View(await utils.GetById(id));
        }
    }
}
