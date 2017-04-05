using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.SystemData;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.Models.DBLoggers
{
    public class DBLoggerRegionModel
    {
        public Datacenter Datacenter { get; set; }

        public List<DBLoggerModel> DBLoggers { get; private set; } = new List<DBLoggerModel>();

        public IEnumerable<Cross> AllSubscribedPairs => DBLoggers.Select(l => l.SubscribedPairs.AsEnumerable()).Aggregate((cur, next) => cur.Concat(next)).Distinct().OrderBy(c => c.ToString());

        public IEnumerable<Cross> AllUnsubscribedPairs => CrossUtils.AllCrosses.Except(AllSubscribedPairs);

        public DBLoggerRegionModel(Datacenter datacenter)
        {
            Datacenter = datacenter;
        }
    }
}
