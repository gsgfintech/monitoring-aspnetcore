using Capital.GSG.FX.Data.Core.ContractData;
using System.Collections.Generic;

namespace StratedgemeMonitor.Models.DBLoggers
{
    public class DBLoggerActionModel
    {
        public string DBLoggerName { get; set; }
        public string Action { get; set; }
        public List<Cross> Crosses { get; set; }
    }
}
