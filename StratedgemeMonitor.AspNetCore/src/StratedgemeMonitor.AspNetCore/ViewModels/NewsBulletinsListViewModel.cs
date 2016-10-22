using StratedgemeMonitor.AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class NewsBulletinsListViewModel
    {
        public List<NewsBulletinModel> OpenBulletins { get; set; }
        public List<NewsBulletinModel> BulletinsClosedToday { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM}")]
        public DateTime Day { get; private set; }

        public NewsBulletinsListViewModel(DateTime day, List<NewsBulletinModel> openBulletins, List<NewsBulletinModel> bulletinsClosedToday)
        {
            Day = day;

            OpenBulletins = openBulletins;
            BulletinsClosedToday = bulletinsClosedToday;
        }
    }
}
