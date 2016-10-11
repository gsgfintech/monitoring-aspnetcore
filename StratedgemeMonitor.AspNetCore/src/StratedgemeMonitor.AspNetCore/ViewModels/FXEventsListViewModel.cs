using StratedgemeMonitor.AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class FXEventsListViewModel
    {
        public List<FXEventModel> CurrentWeeksFXEvents { get; set; }
        public List<FXEventModel> TodaysHighImpactFXEvents { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTimeOffset CurrentWeekStart { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTimeOffset CurrentWeekEnd { get; set; }
    }
}
