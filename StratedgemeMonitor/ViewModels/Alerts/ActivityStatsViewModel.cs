using StratedgemeMonitor.Models.FXEvents;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.ViewModels.Alerts
{
    public class ActivityStatsViewModel
    {
        [Display(Name = "Active orders")]
        public int ActiveOrdersCount { get; set; }

        [Display(Name = "Inactive orders")]
        public int InactiveOrdersCount { get; set; }

        [Display(Name = "Trades")]
        public int TradesCount { get; set; }

        [Display(Name = "High impact events")]
        public int TodaysHighImpactEventsCount { get; set; }

        [Display(Name = "Next event at ")]
        public FXEventModel NextHighImpactEvent { get; set; }
    }
}
