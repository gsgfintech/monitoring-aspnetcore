using Capital.GSG.FX.Data.Core.NewsBulletinData;
using Capital.GSG.FX.Data.Core.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.NewsBulletins;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.NewsBulletins
{
    [Authorize]
    public class NewsBulletinsController : Controller
    {
        private readonly NewsBulletinsControllerUtils utils;

        public NewsBulletinsController(NewsBulletinsControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(day));
        }

        public async Task<IActionResult> Details(NewsBulletinSource source, string bulletinId)
        {
            NewsBulletinModel bulletin = await utils.Get(source, bulletinId);

            if (bulletin != null)
                return View(bulletin);
            else
                return View("Error");
        }

        public async Task<IActionResult> Close(NewsBulletinSource source, string bulletinId)
        {
            GenericActionResult result = await utils.Close(source, bulletinId);

            if (result.Success)
                return View("Index", await utils.CreateListViewModel(utils.CurrentDay));
            else
                return View("Error", result.Message);
        }

        public async Task<IActionResult> CloseAll()
        {
            GenericActionResult result = await utils.CloseAll();

            if (result.Success)
                return View("Index", await utils.CreateListViewModel(utils.CurrentDay));
            else
                return View("Error", result.Message);
        }
    }
}
