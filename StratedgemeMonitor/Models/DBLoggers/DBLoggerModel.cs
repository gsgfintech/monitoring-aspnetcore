using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Utils.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using StratedgemeMonitor.Models.Systems;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.Models.DBLoggers
{
    public class DBLoggerModel
    {
        public string Name { get; private set; }

        public SystemStatusModel Status { get; private set; }

        public List<Cross> SubscribedPairs { get; private set; }

        public SelectList SubscribedPairsSelect { get; private set; }

        public DBLoggerModel(string name, SystemStatusModel status, DBLoggerSubscriptionStatus subscriptionStatus)
        {
            Name = name;
            Status = status;
            SubscribedPairs = subscriptionStatus.SubscribedPairs;

            if (!subscriptionStatus.SubscribedPairs.IsNullOrEmpty())
                SubscribedPairsSelect = new SelectList(subscriptionStatus.SubscribedPairs.Select(c => c.ToString()).OrderBy(c => c));
        }
    }
}
