﻿using Capital.GSG.FX.Data.Core.SystemData;
using Capital.GSG.FX.Utils.Core;
using StratedgemeMonitor.Models.Systems;
using System.Collections.Generic;
using System.Linq;

namespace StratedgemeMonitor.ViewModels.Systems
{
    public class SystemsListViewModel
    {
        public List<SystemStatusModel> Statuses { get; private set; }

        public SystemStatusLevel? OverallStatus { get; private set; }

        public SystemsListViewModel(SystemStatusLevel? overallStatus, IEnumerable<SystemStatusModel> statuses)
        {
            OverallStatus = overallStatus;

            Statuses = !statuses.IsNullOrEmpty() ? statuses.OrderByDescending(s => s.OverallStatus).ToList() : new List<SystemStatusModel>();
        }
    }
}
