using StratedgemeMonitor.Models.FAConfigurations;

namespace StratedgemeMonitor.ViewModels.FAConfigurations
{
    public class FAConfigurationsIndexViewModel
    {
        public FAConfigurationModel Configuration { get; private set; }

        public FAConfigurationsIndexViewModel(FAConfigurationModel configuration)
        {
            Configuration = configuration;
        }
    }
}
