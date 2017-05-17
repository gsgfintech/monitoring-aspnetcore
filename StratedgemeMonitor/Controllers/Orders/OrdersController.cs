using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.Orders;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Orders
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly OrdersControllerUtils utils;

        public OrdersController(OrdersControllerUtils utils)
        {
            this.utils = utils;
        }

        public async Task<IActionResult> Index(DateTime? day)
        {
            return View(await utils.CreateListViewModel(day));
        }

        public async Task<IActionResult> Details(long id)
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
