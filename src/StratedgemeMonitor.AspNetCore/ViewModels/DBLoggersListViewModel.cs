using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.AspNetCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class DBLoggersListViewModel
    {
        public List<DBLoggerModel> DBLoggers { get; private set; }

        public DBLoggerActionModel Action { get; set; }

        public List<Cross> UnsubscribedPairs { get; private set; }

        public DBLoggersListViewModel() { }

        public DBLoggersListViewModel(IEnumerable<DBLoggerModel> dbLoggers = null)
        {
            if (!dbLoggers.IsNullOrEmpty())
            {
                DBLoggers = new List<DBLoggerModel>(dbLoggers);

                List<Cross> allSubscribedPairs = new List<Cross>();

                foreach (var dbLogger in dbLoggers)
                    allSubscribedPairs.AddRange(dbLogger.SubscribedPairs);

                UnsubscribedPairs = new List<Cross>(CrossUtils.AllCrosses.Except(allSubscribedPairs));
            }
            else
            {
                DBLoggers = new List<DBLoggerModel>();
                UnsubscribedPairs = CrossUtils.AllCrosses.ToList();
            }
        }
    }

    public class DBLoggerActionModel
    {
        public string DBLoggerName { get; set; }
        public string Action { get; set; }
        public List<Cross> Crosses { get; set; }
    }
}
