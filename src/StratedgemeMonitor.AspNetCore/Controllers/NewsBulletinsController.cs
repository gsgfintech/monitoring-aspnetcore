using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Data.Core.NewsBulletinData;
using StratedgemeMonitor.AspNetCore.Models;
using Capital.GSG.FX.Data.Core.WebApi;

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
            GenericActionResult result = await utils.Close(source, bulletinId, HttpContext.Session, User);

            if (result.Success)
                return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
            else
                return View("Error", result.Message);
        }

        public async Task<IActionResult> CloseAll()
        {
            GenericActionResult result = await utils.CloseAll(HttpContext.Session, User);

            if (result.Success)
                return View("Index", await utils.CreateListViewModel(HttpContext.Session, User, utils.CurrentDay));
            else
                return View("Error", result.Message);
        }
    }
}
