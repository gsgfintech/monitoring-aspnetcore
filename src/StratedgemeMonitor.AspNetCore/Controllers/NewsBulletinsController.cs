using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Data.Core.NewsBulletinData;
using StratedgemeMonitor.AspNetCore.Models;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class NewsBulletinsController : Controller
    {
        private readonly NewsBulletinsControllerUtils utils;

        public NewsBulletinsController(BackendNewsBulletinsConnector connector)
        {
            utils = new NewsBulletinsControllerUtils(connector);
        }

        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User, day));
        }

        public async Task<IActionResult> Details(NewsBulletinSource source, string bulletinId)
        {
            NewsBulletinModel bulletin = await utils.Get(source, bulletinId, HttpContext.Session, User);

            if (bulletin != null)
                return View(bulletin);
            else
                return View("Error");
        }

        public async Task<IActionResult> Close(NewsBulletinSource source, string bulletinId)
        {
            bool result = await utils.Close(source, bulletinId, HttpContext.Session, User);

            if (result)
                return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
            else
                return View("Error");
        }

        public async Task<IActionResult> CloseAll()
        {
            bool result = await utils.CloseAll(HttpContext.Session, User);

            if (result)
                return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
            else
                return View("Error");
        }
    }
}
