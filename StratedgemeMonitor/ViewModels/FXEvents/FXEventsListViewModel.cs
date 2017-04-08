using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.Models.FXEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.ViewModels.FXEvents
{
    public class FXEventsListViewModel
    {
        public Dictionary<DateTime, List<FXEventModel>> CurrentWeeksFXEvents { get; set; }

        public int FXEventsCount => !CurrentWeeksFXEvents.IsNullOrEmpty() ? CurrentWeeksFXEvents.Select(d => d.Value?.Count ?? 0).Sum() : 0;

        public List<FXEventModel> TodaysHighImpactFXEvents { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTimeOffset CurrentWeekStart { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTimeOffset CurrentWeekEnd { get; set; }
    }
}
