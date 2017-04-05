using StratedgemeMonitor.Models.DBLoggers;
using System.Collections.Generic;

namespace StratedgemeMonitor.ViewModels.DBLoggers
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
}
