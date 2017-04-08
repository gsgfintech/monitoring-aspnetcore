using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.FXEvents
{
    [Authorize]
    public class FXEventsController : Controller
    {
        private readonly FXEventsControllerUtils utils;

        public FXEventsController(FXEventsControllerUtils utils)
        {
            this.utils = utils;
        }

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
