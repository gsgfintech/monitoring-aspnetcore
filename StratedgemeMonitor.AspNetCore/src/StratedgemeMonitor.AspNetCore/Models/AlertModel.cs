using Capital.GSG.FX.Data.Core.SystemData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.Models
{
    public class AlertModel
    {
        public string ActionUrl { get; set; }

        [Display(Name = "ID")]
        public string AlertId { get; set; }

        [Display(Name = "Message")]
        public string Body { get; set; }

        [Display(Name = "Closed Time (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset? ClosedTimestamp { get; set; }

        public AlertLevel Level { get; set; }

        public string Source { get; set; }

        public AlertStatus Status { get; set; }

        [Display(Name = "Header")]
        public string Subject { get; set; }

        [Display(Name = "Timestamp (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset Timestamp { get; set; }
    }

    internal static class AlertModelExtensions
    {
        public static AlertModel ToAlertModel(this Alert alert)
        {
            if (alert == null)
                return null;

            return new AlertModel()
            {
                ActionUrl = alert.ActionUrl,
                AlertId = alert.AlertId,
                Body = alert.Body,
                ClosedTimestamp = alert.ClosedTimestamp,
                Level = alert.Level,
                Source = alert.Source,
                Status = alert.Status,
                Subject = alert.Subject,
                Timestamp = alert.Timestamp
            };
        }

        public static List<AlertModel> ToAlertModels(this IEnumerable<Alert> alerts)
        {
            return alerts?.Select(a => a.ToAlertModel()).ToList();
        }
    }
}
