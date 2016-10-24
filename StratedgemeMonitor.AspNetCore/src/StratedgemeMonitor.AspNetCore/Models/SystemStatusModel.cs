using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Utils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.Models
{
    public class SystemStatusModel
    {
        [Display(Name = "Alive")]
        public bool IsAlive { get; set; }

        [Display(Name = "Last Update (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset LastHeardFrom { get; set; }

        [Display(Name = "System")]
        public string Name { get; set; }

        [Display(Name = "Status")]
        public SystemStatusLevel? OverallStatus { get; set; }

        public bool Restartable { get; set; }

        [Display(Name = "Start Time (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset StartTime { get; set; }

        public List<SystemStatusAttributeModel> Attributes { get; set; }
    }

    public class SystemStatusAttributeModel
    {

        [Display(Name = "Acked Until (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset? AckedUntil { get; set; }

        public SystemStatusLevel Level { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }

    internal static class SystemStatusModelExtensions
    {
        public static SystemStatusModel ToSystemStatusModel(this SystemStatus status)
        {
            if (status == null)
                return null;

            SystemStatusModel model = new SystemStatusModel()
            {
                Attributes = status.Attributes.ToSystemStatusAttributeModels(),
                IsAlive = status.IsAlive,
                LastHeardFrom = status.LastHeardFrom.ToOffset(TimeSpan.FromHours(8)),
                Name = status.Name,
                OverallStatus = status.OverallStatus,
                Restartable = status.Restartable,
                StartTime = status.StartTime.ToOffset(TimeSpan.FromHours(8))
            };

            if (!model.OverallStatus.HasValue)
            {
                if (!model.Attributes.IsNullOrEmpty())
                    model.OverallStatus = SystemStatusLevelUtils.CalculateWorstOf(model.Attributes.Select(a => a.Level));
                else
                    model.OverallStatus = SystemStatusLevel.RED;
            }

            return model;
        }

        public static List<SystemStatusModel> ToSystemStatusModels(this IEnumerable<SystemStatus> statuses)
        {
            return statuses?.Select(s => s.ToSystemStatusModel()).ToList();
        }

        private static SystemStatusAttributeModel ToSystemStatusAttributeModel(this SystemStatusAttribute attribute)
        {
            if (attribute == null)
                return null;

            return new SystemStatusAttributeModel()
            {
                AckedUntil = attribute.AckedUntil,
                Level = attribute.Level,
                Name = attribute.Name,
                Value = attribute.Value
            };
        }

        public static List<SystemStatusAttributeModel> ToSystemStatusAttributeModels(this IEnumerable<SystemStatusAttribute> attributes)
        {
            return attributes?.Select(a => a.ToSystemStatusAttributeModel()).ToList();
        }
    }
}
