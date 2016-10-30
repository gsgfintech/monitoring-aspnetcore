using Capital.GSG.FX.Monitoring.Server.Connector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.AspNetCore.Models;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly OrdersControllerUtils utils;

        public OrdersController(BackendOrdersConnector connector)
        {
            utils = new OrdersControllerUtils(connector);
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(HttpContext.Session, User, day));
        }

        public async Task<IActionResult> Details(int id)
        {
            OrderModel order = await utils.GetByPermanentId(id, HttpContext.Session, User);

            if (order != null)
                return View(order);
            else
                return View("Error");
        }
    }
}
