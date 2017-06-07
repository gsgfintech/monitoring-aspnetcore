using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratedgemeMonitor.Models.Stratedgeme.Strategy;
using Syncfusion.JavaScript;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Stratedgeme.Strategies
{
    [Authorize]
    [Route("api/strategies")]
    public class StrategiesApiController : Controller
    {
        private readonly StrategiesControllerUtils utils;

        public StrategiesApiController(StrategiesControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpPost("edit/list-params/{name}/{version}")]
        public async Task<IActionResult> EditListParamsForDataGrid(string name, string version)
        {
            var result = await utils.Get(name, version);

            var data = result.Model?.Config?.ToList() ?? new List<ConfigParamModel>();

            return Json(new { result = data, count = data.Count });
        }

        [HttpPost("edit/add-param/{name}/{version}")]
        public async Task<IActionResult> EditAddParam([FromBody]CRUDModel<ConfigParamModel> value, string name, string version)
        {
            if (value == null || value.Value == null)
                return Ok();

            var result = await utils.AddConfigParam(name, version, value.Value);

            // TODO : use result

            return await EditListParamsForDataGrid(name, version);
        }

        [HttpPost("edit/update-param/{name}/{version}")]
        public async Task<IActionResult> EditUpdateParam([FromBody]CRUDModel<ConfigParamModel> value, string name, string version)
        {
            if (value == null || value.Value == null)
                return Ok();

            var result = await utils.UpdateConfigParam(name, version, value.Value);

            // TODO : use result

            return await EditListParamsForDataGrid(name, version);
        }

        [HttpPost("edit/delete-param/{name}/{version}")]
        public async Task<IActionResult> EditDeleteParam([FromBody]CRUDModel key, string name, string version)
        {
            var result = await utils.DeleteConfigParam(name, version, key.Key.ToString());

            // TODO : use result

            return await EditListParamsForDataGrid(name, version);
        }

        [HttpPost("edit/list-crosses/{name}/{version}")]
        public async Task<IActionResult> EditListCrossesForDataGrid(string name, string version)
        {
            var result = await utils.Get(name, version);

            var data = result.Model?.CrossesConfig?.ToList() ?? new List<CrossConfigModel>();

            return Json(new { result = data, count = data.Count });
        }

        [HttpPost("edit/add-cross/{name}/{version}")]
        public async Task<IActionResult> EditAddCross([FromBody]CRUDModel<CrossConfigModel> value, string name, string version)
        {
            if (value == null || value.Value == null)
                return Ok();

            var result = await utils.AddCrossParam(name, version, value.Value);

            return await EditListCrossesForDataGrid(name, version);
        }

        [HttpPost("edit/update-cross/{name}/{version}")]
        public async Task<IActionResult> EditUpdateCross([FromBody]CRUDModel<CrossConfigModel> value, string name, string version)
        {
            if (value == null || value.Value == null)
                return Ok();

            var result = await utils.UpdateCrossParam(name, version, value.Value);

            return await EditListCrossesForDataGrid(name, version);
        }

        [HttpPost("edit/delete-cross/{name}/{version}")]
        public async Task<IActionResult> EditDeleteCross([FromBody]CRUDModel key, string name, string version)
        {
            var result = await utils.DeleteCrossParam(name, version, key.Key.ToString());

            return await EditListCrossesForDataGrid(name, version);
        }
    }
}
