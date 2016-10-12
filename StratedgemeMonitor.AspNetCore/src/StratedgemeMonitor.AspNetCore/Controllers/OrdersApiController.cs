using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StratedgemeMonitor.AspNetCore.Models;
using Capital.GSG.FX.Data.Core.OrderData;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    [Authorize]
    [Route("api/orders")]
    public class OrdersApiController : Controller
    {
        private readonly OrdersControllerUtils utils;

        public OrdersApiController(MonitorDbContext db)
        {
            utils = new OrdersControllerUtils(db);
        }

        // GET: api/values
        [HttpGet]
        public async Task<List<OrderModel>> Get(DateTime day)
        {
            return await utils.GetOrdersForDay(day);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<OrderModel> Get(int id)
        {
            return await utils.GetByPermanentId(id);
        }

        // POST api/values
        [HttpPost]
        public async Task<bool> Post([FromBody]Order order)
        {
            return await utils.AddOrUpdate(order);
        }
    }
}
