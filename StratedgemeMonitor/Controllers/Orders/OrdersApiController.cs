using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Orders
{
    [Authorize]
    [Route("api/orders")]
    public class OrdersApiController : Controller
    {
        private readonly OrdersControllerUtils utils;

        public OrdersApiController(OrdersControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpGet("inactivate/{broker}/{permanentId}")]
        public async Task<GenericActionResult> MarkAsInactive(Broker broker, long permanentId)
        {
            var result = await utils.MarkAsInactive(broker, permanentId);

            return new GenericActionResult(result.Success, result.Message);
        }
    }
}
