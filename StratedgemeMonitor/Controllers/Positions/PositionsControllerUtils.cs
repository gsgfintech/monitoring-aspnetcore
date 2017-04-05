using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.Models.Accounts;
using StratedgemeMonitor.Models.Positions;
using StratedgemeMonitor.ViewModels.Positions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Positions
{
    public class PositionsControllerUtils
    {
        private readonly BackendAccountsConnector accountsConnector;
        private readonly BackendPositionsConnector positionsConnector;

        public PositionsControllerUtils(BackendPositionsConnector positionsConnector, BackendAccountsConnector accountsConnector)
        {
            this.accountsConnector = accountsConnector;
            this.positionsConnector = positionsConnector;
        }

        internal async Task<PositionsListViewModel> CreateListViewModel()
        {
            List<AccountModel> accounts = await GetAllAccounts();
            var positions = await GetAllPositions();

            return new PositionsListViewModel(positions, accounts ?? new List<AccountModel>());
        }

        private async Task<Dictionary<string, List<PositionModel>>> GetAllPositions()
        {
            var positions = await positionsConnector.GetAll();

            return positions.ToPositionModelsDict();
        }

        internal async Task<PositionModel> Get(Broker broker, Cross cross)
        {
            return (await positionsConnector.Get(broker, cross)).ToPositionModel();
        }

        private async Task<List<AccountModel>> GetAllAccounts()
        {
            var positions = await accountsConnector.GetAll();

            return positions.ToAccountModels();
        }

        internal async Task<AccountModel> GetAccount(Broker broker, string accountName)
        {
            return (await accountsConnector.Get(broker, accountName)).ToAccountModel();
        }
    }
}
