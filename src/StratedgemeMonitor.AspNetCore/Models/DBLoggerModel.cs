﻿using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Utils.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.AspNetCore.Models
{
    public class DBLoggerModel
    {
        public string Name { get; private set; }

        public SystemStatusModel Status { get; private set; }

        public DBLoggerConfigModel Config { get; private set; }

        public SelectList SubscribedPairs { get; private set; }

        public DBLoggerModel(string name, SystemStatusModel status, DBLoggerConfigModel config)
        {
            Name = name;
            Status = status;
            Config = config;

            if (!config.Pairs.IsNullOrEmpty())
                SubscribedPairs = new SelectList(config.Pairs.Select(c => c.ToString()).OrderBy(c => c));
        }
    }

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
