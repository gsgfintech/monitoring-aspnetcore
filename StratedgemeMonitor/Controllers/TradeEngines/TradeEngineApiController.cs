using Capital.GSG.FX.Data.Core.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.TradeEngines
{
    [Authorize]
    [Route("api/tradeengine")]
    public class TradeEngineApiController : Controller
    {
        private readonly TradeEngineControllerUtils utils;

        public TradeEngineApiController(TradeEngineControllerUtils utils)
        {
            this.utils = utils;
        }

        [HttpGet("{tradeEngineName}/resetpositionstatusstrategy/{strat}")]
        public async Task<(bool Success, string Message)> RequestStratToResetPositionStatus(string tradeEngineName, string strat)
        {
            return await utils.RequestStratToResetPositionStatus(tradeEngineName, strat);
        }

        [HttpGet("{tradeEngineName}/resettradingconnectionstatus/{isConnected}")]
        public async Task<(bool Success, string Message)> ResetTradingConnectionStatus(string tradeEngineName, bool isConnected)
        {
            return await utils.ResetTradingConnectionStatus(tradeEngineName, isConnected);
        }

        [HttpGet("{tradeEngineName}/starttrading/{stratName}/{stratVersion}/{cross}")]
        public async Task<GenericActionResult> StartTrading(string tradeEngineName, string stratName, string stratVersion, string cross)
        {
            var result = await utils.StartTrading(tradeEngineName, stratName, stratVersion, cross);

            return new GenericActionResult(result.Success, result.Message);
        }

        [HttpGet("{tradeEngineName}/stoptrading/{stratName}/{stratVersion}/{cross}")]
        public async Task<GenericActionResult> StopTrading(string tradeEngineName, string stratName, string stratVersion, string cross)
        {
            var result = await utils.StopTrading(tradeEngineName, stratName, stratVersion, cross);

            return new GenericActionResult(result.Success, result.Message);
        }
    }
}
