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
        private readonly BackendAccountsConnector accountsConnector;
        private readonly BackendPositionsConnector positionsConnector;

        public PositionsControllerUtils(BackendPositionsConnector positionsConnector, BackendAccountsConnector accountsConnector)
        {
            this.accountsConnector = accountsConnector;
            this.positionsConnector = positionsConnector;
        }

        internal async Task<PositionsListViewModel> CreateListViewModel(ISession session, ClaimsPrincipal user)
        {
            List<AccountModel> accounts = await GetAllAccounts(session, user);
            List<PositionModel> positions = await GetAllPositions(session, user);

            return new PositionsListViewModel(positions ?? new List<PositionModel>(), accounts ?? new List<AccountModel>());
        }

        private async Task<List<PositionModel>> GetAllPositions(ISession session, ClaimsPrincipal user)
        {
            var positions = await positionsConnector.GetAll();

            return positions.ToPositionModels();
        }

        internal async Task<PositionModel> Get(Broker broker, Cross cross, ISession session, ClaimsPrincipal user)
        {
            return (await positionsConnector.Get(broker, cross)).ToPositionModel();
        }

        private async Task<List<AccountModel>> GetAllAccounts(ISession session, ClaimsPrincipal user)
        {
            var positions = await accountsConnector.GetAll();

            return positions.ToAccountModels();
        }

        internal async Task<AccountModel> GetAccount(Broker broker, string accountName, ISession session, ClaimsPrincipal user)
        {
            return (await accountsConnector.Get(broker, accountName)).ToAccountModel();
        }
    }
}
