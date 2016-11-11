using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class AlertsListViewModel
    {
        public List<SystemStatusModel> SystemStatuses { get; set; }
        public List<AlertModel> OpenAlerts { get; set; }
        public List<AlertModel> ClosedAlerts { get; set; }
        public PnLModel PnL { get; set; }

        public SystemStatusLevel? AllSystemsStatus
        {
            get
            {
                if (SystemStatuses.IsNullOrEmpty())
                    return null;

                if (SystemStatuses.Where(s => !s.IsAlive).Count() > 0)
                    return SystemStatusLevel.RED;

                return SystemStatusLevelUtils.CalculateWorstOf(SystemStatuses.Select(s => s.OverallStatus ?? SystemStatusLevel.GREEN));
            }
        }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTime Day { get; private set; }

        public AlertsListViewModel(DateTime day, List<AlertModel> openAlerts, List<AlertModel> closedAlerts, List<SystemStatusModel> systemStatuses, PnLModel pnl)
        {
            Day = day;

            OpenAlerts = openAlerts;
            ClosedAlerts = closedAlerts;
            SystemStatuses = systemStatuses;
            PnL = pnl;
        }
    }
}
