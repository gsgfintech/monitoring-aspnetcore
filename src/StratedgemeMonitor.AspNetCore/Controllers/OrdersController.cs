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
            utils = OrdersControllerUtils.GetInstance(connector);
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(day));
        }

        public async Task<IActionResult> Details(int id)
        {
            OrderModel order = await utils.GetByPermanentId(id);

            if (order != null)
                return View(order);
            else
                return View("Error");
        }

        public async Task<FileResult> ExportExcel()
        {
            return await utils.ExportExcel();
        }
    }
}
