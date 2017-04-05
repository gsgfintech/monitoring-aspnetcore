using Capital.GSG.FX.Data.Core.NewsBulletinData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.Models.NewsBulletins
{
    public class NewsBulletinModel
    {
        [Display(Name = "Type")]
        public NewsBulletinType BulletinType { get; set; }

        [Display(Name = "Closed Time (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset? ClosedTimestamp { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public string Origin { get; set; }

        public NewsBulletinSource Source { get; set; }

        public NewsBulletinStatus Status { get; set; }

        [Display(Name = "Timestamp (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset Timestamp { get; set; }

        public override string ToString()
        {
            return $"{Source} - {Id}";
        }
    }

    internal static class NewsBulletinModelExtensions
    {
        public static NewsBulletinModel ToNewsBulletinModel(this NewsBulletin bulletin)
        {
            if (bulletin == null)
                return null;

            return new NewsBulletinModel()
            {
                BulletinType = bulletin.BulletinType,
                ClosedTimestamp = bulletin.ClosedTimestamp?.ToOffset(TimeSpan.FromHours(8)),
                Id = bulletin.Id,
                Message = bulletin.Message,
                Origin = bulletin.Origin,
                Source = bulletin.Source,
                Status = bulletin.Status,
                Timestamp = bulletin.Timestamp.ToOffset(TimeSpan.FromHours(8))
            };
        }

        public static List<NewsBulletinModel> ToNewsBulletinModels(this IEnumerable<NewsBulletin> bulletins)
        {
            return bulletins?.Select(b => b.ToNewsBulletinModel()).ToList();
        }
    }
}
