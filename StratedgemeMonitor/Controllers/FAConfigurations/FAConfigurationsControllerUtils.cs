﻿using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.Models.FAConfigurations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.FAConfigurations
{
    public class FAConfigurationsControllerUtils
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<FAConfigurationsControllerUtils>();

        private readonly BackendFAConfigurationsConnector connector;

        public FAConfigurationsControllerUtils(BackendFAConfigurationsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<FAConfigurationModel> Get()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var result = await connector.RequestFAConfiguration(cts.Token);

            if (!result.Success)
                logger.Error(result.Message);

            return result.FAConfiguration.ToFAConfigurationModel();
        }
    }
}
