using System;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.ViewModels.Alerts
{
    public class AlertsViewModel
    {
        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTime Day { get; private set; }

        public AlertsViewModel(DateTime day)
        {
            Day = day;
        }
    }
}
