using Capital.GSG.FX.Data.Core.NewsBulletinData;
using Capital.GSG.FX.Data.Core.WebApi;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Microsoft.AspNetCore.Http;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.Utils;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    internal class NewsBulletinsControllerUtils
    {
        private readonly BackendNewsBulletinsConnector connector;

        internal DateTime? CurrentDay { get; private set; }

        public NewsBulletinsControllerUtils(BackendNewsBulletinsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<NewsBulletinsListViewModel> CreateListViewModel(ISession session, ClaimsPrincipal user, DateTime? day = null)
        {
            if (!CurrentDay.HasValue)
                CurrentDay = DateTime.Today;

            if (!day.HasValue)
                day = CurrentDay;
            else
                CurrentDay = day;

            List<NewsBulletinModel> openBulletins = await GetOpenBulletins(session, user);
            List<NewsBulletinModel> bulletinsClosedToday = await GetBulletinsClosedOnDay(day.Value, session, user);

            return new NewsBulletinsListViewModel(day.Value, openBulletins ?? new List<NewsBulletinModel>(), bulletinsClosedToday ?? new List<NewsBulletinModel>());
        }

        internal async Task<GenericActionResult> Close(NewsBulletinSource source, string bulletinId, ISession session, ClaimsPrincipal user)
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

        private async Task<List<NewsBulletinModel>> GetOpenBulletins(ISession session, ClaimsPrincipal user)
        {
            var bulletins = await connector.GetByStatus(NewsBulletinStatus.OPEN);

            return bulletins.ToNewsBulletinModels();
        }

        private async Task<List<NewsBulletinModel>> GetBulletinsClosedOnDay(DateTime day, ISession session, ClaimsPrincipal user)
        {
            var bulletins = await connector.GetForDay(day);

            return bulletins?.Where(b => b.Status == NewsBulletinStatus.CLOSED).ToNewsBulletinModels();
        }

        internal async Task<NewsBulletinModel> Get(NewsBulletinSource source, string id, ISession session, ClaimsPrincipal user)
        {
            return (await connector.Get(source, id)).ToNewsBulletinModel();
        }

        internal async Task<GenericActionResult> CloseAll(ISession session, ClaimsPrincipal user)
        {
            return await connector.CloseAll();
        }
    }
}
