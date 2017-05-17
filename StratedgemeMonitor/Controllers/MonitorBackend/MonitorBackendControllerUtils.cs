using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.Models.MonitorBackend;
using System;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.MonitorBackend
{
    public class MonitorBackendControllerUtils
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<MonitorBackendControllerUtils>();

        private readonly BackendExecutionsConnector executionsConnector;

        public DateTimeOffset? LastTradesDictResetTime { get; set; }

        public MonitorBackendControllerUtils(BackendExecutionsConnector executionsConnector)
        {
            this.executionsConnector = executionsConnector;
        }

        internal async Task<MonitorBackendModel> CreateMonitorBackendModel()
        {
            LastTradesDictResetTime = await executionsConnector.GetLastTradesDictionaryResetTime();

            return new MonitorBackendModel()
            {
                ResetTradesDictModel = new ResetTradesDictModel()
                {
                    LastTradesDictResetTime = LastTradesDictResetTime
                }
            };
        }

        internal ResetTradesDictModel CreateResetTradesDictModel()
        {
            return new ResetTradesDictModel()
            {
                LastTradesDictResetTime = LastTradesDictResetTime
            };
        }

        internal async Task<(bool, string)> ResetTradesDictionary()
        {
            logger.Info("Requesting to reset trades dictionary");

            var result = await executionsConnector.ResetTradesDictionary();

            if (result.Item1)
                LastTradesDictResetTime = DateTimeOffset.Now;

            return result;
        }
    }
}
