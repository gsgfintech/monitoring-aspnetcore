using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
