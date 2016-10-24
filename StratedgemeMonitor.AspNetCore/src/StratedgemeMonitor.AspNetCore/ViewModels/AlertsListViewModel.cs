using StratedgemeMonitor.AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class AlertsListViewModel
    {
        public List<SystemStatusModel> SystemStatuses { get; set; }
        public List<AlertModel> OpenAlerts { get; set; }
        public List<AlertModel> ClosedAlerts { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTime Day { get; private set; }

        public AlertsListViewModel(DateTime day, List<AlertModel> openAlerts, List<AlertModel> closedAlerts, List<SystemStatusModel> systemStatuses)
        {
            Day = day;

            OpenAlerts = openAlerts;
            ClosedAlerts = closedAlerts;
            SystemStatuses = systemStatuses;
        }
    }
}
