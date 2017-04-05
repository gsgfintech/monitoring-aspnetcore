using StratedgemeMonitor.AspNetCore.Models;
using System.Collections.Generic;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class PositionsListViewModel
    {
        public List<AccountModel> Accounts { get; set; }
        public Dictionary<string, List<PositionModel>> Positions { get; set; }

        public PositionsListViewModel(Dictionary<string, List<PositionModel>> positions, List<AccountModel> accounts)
        {
            Accounts = accounts;
            Positions = positions;
        }
    }
}
