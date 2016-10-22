using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Microsoft.AspNetCore.Http;
using StratedgemeMonitor.AspNetCore.Models;
using StratedgemeMonitor.AspNetCore.Utils;
using StratedgemeMonitor.AspNetCore.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StratedgemeMonitor.AspNetCore.Controllers
{
    internal class PositionsControllerUtils
    {
        private readonly BackendPositionsConnector connector;

        public PositionsControllerUtils(BackendPositionsConnector connector)
        {
            this.connector = connector;
        }

        internal async Task<PositionsListViewModel> CreateListViewModel(ISession session, ClaimsPrincipal user)
        {
            List<PositionModel> positions = await GetAllPositions(session, user);

            return new PositionsListViewModel(positions ?? new List<PositionModel>());
        }

        private async Task<List<PositionModel>> GetAllPositions(ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            var positions = await connector.GetAll(accessToken);

            return positions.ToPositionModels();
        }

        internal async Task<PositionModel> Get(Broker broker, Cross cross, ISession session, ClaimsPrincipal user)
        {
            string accessToken = await AzureADAuthenticator.RetrieveAccessToken(user, session);

            return (await connector.Get(broker, cross, accessToken)).ToPositionModel();
        }
    }
}
