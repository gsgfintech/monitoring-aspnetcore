using System;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.Models.MonitorBackend
{
    public class ResetTradesDictModel
    {
        [Display(Name = "Last Trades Dictionary Reset Time")]
        public DateTimeOffset? LastTradesDictResetTime { get; set; }
    }
}
