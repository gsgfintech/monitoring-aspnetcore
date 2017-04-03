using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers.Systems
{
    public class SystemsControllerUtils
    {
        private readonly BackendSystemStatusesConnector systemStatusesConnector;
        private readonly BackendSystemServicesConnector systemServicesConnector;

        public SystemsControllerUtils(BackendSystemStatusesConnector systemStatusesConnector, BackendSystemServicesConnector systemServicesConnector)
        {
            this.systemStatusesConnector = systemStatusesConnector;
            this.systemServicesConnector = systemServicesConnector;
        }

        internal async Task<GenericActionResult> StartSystem(string systemName)
        {
            return await systemServicesConnector.StartService(systemName);
        }

        internal async Task<GenericActionResult> StopSystem(string systemName)
        {
            return await systemServicesConnector.StopService(systemName);
        }

        internal async Task<GenericActionResult> SystemDelete(string systemName)
        {
            return await systemStatusesConnector.Delete(systemName);
        }
    }
}
