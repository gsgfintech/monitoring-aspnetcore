using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.SystemData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.Models.DBLoggers
{
    public class DBLoggerConfigModel : ISystemConfig
    {
        public string Name { get; set; }
        public string SystemType { get; set; }
        public List<Cross> Pairs { get; set; }
    }

    internal static class DBLoggerConfigModelExtensions
    {
        private static DBLoggerConfigModel ToDBLoggerConfigModel(this string json)
        {
            return JsonConvert.DeserializeObject<DBLoggerConfigModel>(json, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
        }

        public static IEnumerable<DBLoggerConfigModel> ToDBLoggerConfigModels(this IEnumerable<string> jsons)
        {
            return jsons?.Select(j => j.ToDBLoggerConfigModel());
        }
    }
}
