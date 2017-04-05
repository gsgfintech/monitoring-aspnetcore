using System.Collections.Generic;
using Capital.GSG.FX.Data.Core.SystemData;
using StratedgemeMonitor.Models.Systems;

namespace StratedgemeMonitor.ViewComponents
{
    internal class SystemsListViewModel
    {
        private SystemStatusLevel? overallStatus;
        private List<SystemStatusModel> statuses;

        public SystemsListViewModel(SystemStatusLevel? overallStatus, List<SystemStatusModel> statuses)
        {
            this.overallStatus = overallStatus;
            this.statuses = statuses;
        }
    }
}