using StratedgemeMonitor.AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.AspNetCore.ViewModels
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
