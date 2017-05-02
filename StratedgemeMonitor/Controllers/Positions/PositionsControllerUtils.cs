using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.Extensions.Logging;
using StratedgemeMonitor.Models.Accounts;
using StratedgemeMonitor.Models.Positions;
using StratedgemeMonitor.ViewModels.Positions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StratedgemeMonitor.Controllers.Positions
{
    public class PositionsControllerUtils
    {
        private readonly ILogger logger = GSGLoggerFactory.Instance.CreateLogger<PositionsControllerUtils>();

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
            var result = await positionsConnector.GetAll();

            if (!result.Success)
                logger.Error(result.Message);

            return result.Positions.ToPositionModelsDict();
        }

        internal async Task<PositionModel> Get(Broker broker, string account, Cross cross)
        {
            var result = await positionsConnector.Get(broker, account, cross);

            if (!result.Success)
                logger.Error(result.Message);

            return result.Position.ToPositionModel();
        }

        private async Task<List<AccountModel>> GetAllAccounts()
        {
            var result = await accountsConnector.GetAll();

            if (result.Success)
                return result.Accounts.ToAccountModels();
            else
                return null;
        }

        internal async Task<AccountModel> GetAccount(Broker broker, string accountName)
        {
            var result = await accountsConnector.Get(broker, accountName);

            if (result.Success)
                return result.Account.ToAccountModel();
            else
                return null;
        }
    }
}
