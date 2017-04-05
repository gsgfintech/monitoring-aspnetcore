using Capital.GSG.FX.Data.Core.NewsBulletinData;
using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.Models.NewsBulletins;
using StratedgemeMonitor.ViewModels.NewsBulletins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.NewsBulletins
{
    public class NewsBulletinsControllerUtils
    {
        private readonly BackendNewsBulletinsConnector connector;

        internal DateTime? CurrentDay { get; private set; }

        public NewsBulletinsControllerUtils(BackendNewsBulletinsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<NewsBulletinsListViewModel> CreateListViewModel(DateTime? day = null)
        {
            if (!CurrentDay.HasValue)
                CurrentDay = DateTime.Today;

            if (!day.HasValue)
                day = CurrentDay;
            else
                CurrentDay = day;

            List<NewsBulletinModel> openBulletins = await GetOpenBulletins();
            List<NewsBulletinModel> bulletinsClosedToday = await GetBulletinsClosedOnDay(day.Value);

            return new NewsBulletinsListViewModel(day.Value, openBulletins ?? new List<NewsBulletinModel>(), bulletinsClosedToday ?? new List<NewsBulletinModel>());
        }

        internal async Task<GenericActionResult> Close(NewsBulletinSource source, string bulletinId)
        {
            var bulletin = await connector.Get(source, bulletinId);

            if (bulletin != null)
            {
                bulletin.Status = NewsBulletinStatus.CLOSED;
                bulletin.ClosedTimestamp = DateTimeOffset.Now;

                return await connector.AddOrUpdate(bulletin);
            }
            else
                return new GenericActionResult(false, "Bulletin is null");
        }

        private async Task<List<NewsBulletinModel>> GetOpenBulletins()
        {
            var bulletins = await connector.GetByStatus(NewsBulletinStatus.OPEN);

            return bulletins.ToNewsBulletinModels();
        }

        private async Task<List<NewsBulletinModel>> GetBulletinsClosedOnDay(DateTime day)
        {
            var bulletins = await connector.GetForDay(day);

            return bulletins?.Where(b => b.Status == NewsBulletinStatus.CLOSED).ToNewsBulletinModels();
        }

        internal async Task<NewsBulletinModel> Get(NewsBulletinSource source, string id)
        {
            return (await connector.Get(source, id)).ToNewsBulletinModel();
        }

        internal async Task<GenericActionResult> CloseAll()
        {
            return await connector.CloseAll();
        }
    }
}
