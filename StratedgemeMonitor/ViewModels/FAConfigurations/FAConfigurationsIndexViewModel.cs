using StratedgemeMonitor.Models.FAConfigurations;
using System.Collections.Generic;

namespace StratedgemeMonitor.ViewModels.FAConfigurations
{
    public class FAConfigurationsIndexViewModel
    {
        public List<FAConfigurationModel> Configurations { get; private set; }

        public FAConfigurationsIndexViewModel(List<FAConfigurationModel> configurations)
        {
            Configurations = configurations ?? new List<FAConfigurationModel>();
        }
    }
}
