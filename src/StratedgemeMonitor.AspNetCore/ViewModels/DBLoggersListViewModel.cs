using Capital.GSG.FX.Data.Core.ContractData;
using StratedgemeMonitor.AspNetCore.Models;
using System.Collections.Generic;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class DBLoggersListViewModel
    {
        public List<DBLoggerRegionModel> DBLoggerRegions { get; private set; }

        public DBLoggerActionModel Action { get; set; }

        public DBLoggersListViewModel() { }

        public DBLoggersListViewModel(List<DBLoggerRegionModel> dbLoggerRegions = null)
        {
            DBLoggerRegions = dbLoggerRegions ?? new List<DBLoggerRegionModel>();
        }
    }

    public class DBLoggerActionModel
    {
        public string DBLoggerName { get; set; }
        public string Action { get; set; }
        public List<Cross> Crosses { get; set; }
    }
}
