using StratedgemeMonitor.AspNetCore.Models;
using System.Collections.Generic;

namespace StratedgemeMonitor.AspNetCore.ViewModels
{
    public class PositionsListViewModel
    {
        public List<PositionModel> Positions { get; set; }

        public PositionsListViewModel(List<PositionModel> positions)
        {
            Positions = positions;
        }
    }
}
