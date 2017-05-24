using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.IB.FutureContracts;
using Syncfusion.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.IB.FutureContracts
{
    [Authorize]
    [Route("api/futurecontracts")]
    public class FutureContractsApiController : Controller
    {
        private readonly FutureContractsControllerUtils utils;

        public FutureContractsApiController(FutureContractsControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var result = await utils.GetAll();

            var data = result.Contracts ?? new List<FutureContractModel>();

            return Json(new { result = data, count = data.Count });
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody]CRUDModel<FutureContractModel> value)
        {
            if (value == null || value.Value == null)
                return Ok();

            var result = await utils.AddOrUpdate(value.Value);

            return await List();
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody]CRUDModel<FutureContractModel> value)
        {
            if (value == null || value.Value == null)
                return Ok();

            var result = await utils.AddOrUpdate(value.Value);

            // TODO : use result

            return await List();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody]CRUDModel key)
        {
            //var result = await utils.Delete(key.Key.ToString());

            // TODO : use result

            return await List();
        }
    }
}
