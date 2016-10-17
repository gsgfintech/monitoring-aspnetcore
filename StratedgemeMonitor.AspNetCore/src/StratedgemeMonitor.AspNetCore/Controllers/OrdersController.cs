using Capital.GSG.FX.Data.Core.OrderData;
using Capital.GSG.FX.MongoConnector.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly OrdersControllerUtils utils;

        public OrdersController(MongoDBServer mongoDBServer)
        {
            utils = new OrdersControllerUtils(mongoDBServer);
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(day));
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await utils.GetByPermanentId(id));
        }
    }
}
