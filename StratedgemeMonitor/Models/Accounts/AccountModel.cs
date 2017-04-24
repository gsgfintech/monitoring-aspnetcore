using Capital.GSG.FX.Data.Core.AccountPortfolio;
using Capital.GSG.FX.Data.Core.ContractData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.Models.Accounts
{
    public class AccountModel
    {
        public Broker Broker { get; set; }

        [Display(Name = "Last Update (HKT)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM HH:mm:ss}")]
        public DateTimeOffset LastUpdate { get; set; }

        [Display(Name = "Account")]
        public string Name { get; set; }

        [Display(Name = "Base Currency")]
        public Currency BaseCurrency { get; set; }

        [Display(Name = "Available Funds")]
        public AccountComplexDoubleAttributeModel AvailableFunds { get; set; }

        [Display(Name = "Leverage")]
        public AccountComplexDoubleAttributeModel Leverage { get; set; }
    }

    public class AccountComplexDoubleAttributeModel
    {
        [Display(Name = "Commodities")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double CommoditiesValue { get; set; }

        [Display(Name = "Stocks")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double StocksValue { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TotalValue { get; set; }

        public Currency Currency { get; set; }
    }

    internal static class AccountModelExtensions
    {
        private static AccountComplexDoubleAttributeModel ToAccountComplexDoubleAttributeModel(this AccountComplexDoubleAttribute attribute)
        {
            if (attribute == null)
                return null;

            return new AccountComplexDoubleAttributeModel()
            {
                CommoditiesValue = attribute.CommoditiesValue,
                Currency = attribute.Currency,
                StocksValue = attribute.StocksValue,
                TotalValue = attribute.TotalValue
            };
        }

        public static AccountModel ToAccountModel(this Account account)
        {
            if (account == null)
                return null;

            return new AccountModel()
            {
                AvailableFunds = account.AvailableFunds.ToAccountComplexDoubleAttributeModel(),
                BaseCurrency = account.BaseCurrency,
                Broker = account.Broker,
                Leverage = account.Leverage.ToAccountComplexDoubleAttributeModel(),
                LastUpdate = account.LastUpdate,
                Name = account.Name
            };
        }

        public static List<AccountModel> ToAccountModels(this IEnumerable<Account> accounts)
        {
            return accounts?.Select(a => a.ToAccountModel()).ToList();
        }
    }
}
