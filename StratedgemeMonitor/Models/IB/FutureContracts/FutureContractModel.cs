using Capital.GSG.FX.Data.Core.ContractData;
using IBData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.Models.IB.FutureContracts
{
    public class FutureContractModel
    {
        public string Symbol { get; set; }
        public string Currency { get; set; }

        [Display(Name = "Next Expiration")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CurrentExpi { get; set; }

        public string Description { get; set; }
        public string Exchange { get; set; }
        public double Multiplier { get; set; }
        public string StrKey { get; set; }
    }

    public static class FutureContractModelExtensions
    {
        public static FutureContractModel ToFutureContractModel(this FutureContract contract)
        {
            if (contract == null)
                return null;

            return new FutureContractModel()
            {
                Currency = contract.Currency.ToString(),
                CurrentExpi = contract.CurrentExpi,
                Description = contract.Description,
                Exchange = contract.Exchange,
                Multiplier = contract.Multiplier,
                StrKey = $"{contract.Exchange}_{contract.Symbol}_{contract.Multiplier}",
                Symbol = contract.Symbol
            };
        }

        public static List<FutureContractModel> ToFutureContractModels(this IEnumerable<FutureContract> contracts)
        {
            return contracts?.Select(c => c.ToFutureContractModel()).ToList();
        }

        public static FutureContract ToFutureContract(this FutureContractModel contract)
        {
            if (contract == null)
                return null;

            return new FutureContract()
            {
                Currency = CurrencyUtils.GetFromStr(contract.Currency),
                CurrentExpi = contract.CurrentExpi,
                Description = contract.Description,
                Exchange = contract.Exchange,
                Multiplier = contract.Multiplier,
                Symbol = contract.Symbol
            };
        }

        public static List<FutureContract> ToFutureContracts(this IEnumerable<FutureContractModel> contracts)
        {
            return contracts?.Select(c => c.ToFutureContract()).ToList();
        }
    }
}
