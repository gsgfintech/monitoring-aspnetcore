using Capital.GSG.FX.Monitoring.Server.Connector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class FXEventsController : Controller
    {
        private readonly FXEventsControllerUtils utils;

        public FXEventsController(BackendFXEventsConnector connector)
        {
            utils = new FXEventsControllerUtils(connector);
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User));
        }

        public async Task<IActionResult> Details(string id)
        {
            return View(await utils.GetById(id, HttpContext.Session, User));
        }
    }
}
