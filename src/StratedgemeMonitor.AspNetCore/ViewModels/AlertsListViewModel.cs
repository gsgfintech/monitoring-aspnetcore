using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.ViewComponents;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class AlertsListViewModel
    {
        public AlertsListType ListType { get; private set; }

        public List<AlertModel> Alerts { get; private set; }

        public string Header { get; set; }

        public AlertsListViewModel(AlertsListType listType, string header, IEnumerable<AlertModel> alerts)
        {
            ListType = listType;
            Header = header;
            Alerts = !alerts.IsNullOrEmpty() ? alerts.OrderByDescending(a => a.Timestamp).ToList() : new List<AlertModel>();
        }
    }
}
