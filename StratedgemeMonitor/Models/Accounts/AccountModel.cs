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

        [Display(Name = "Cash Balance")]
        public string CashBalance { get; set; }

        [Display(Name = "Full Available Funds")]
        public AccountComplexDoubleAttributeModel FullAvailableFunds { get; set; }

        [Display(Name = "Full Excess Liquidity")]
        public AccountComplexDoubleAttributeModel FullExcessLiquidity { get; set; }

        [Display(Name = "Full Init Margin Requirement")]
        public AccountComplexDoubleAttributeModel FullInitMarginReq { get; set; }

        [Display(Name = "Full Maintenance Margin Requirement")]
        public AccountComplexDoubleAttributeModel FullMaintMarginReq { get; set; }

        [Display(Name = "Leverage")]
        public AccountComplexDoubleAttributeModel Leverage { get; set; }

        [Display(Name = "Realized PnL")]
        public string RealizedPnL { get; set; }

        [Display(Name = "Total Cash Balance")]
        public string TotalCashBalance { get; set; }

        [Display(Name = "Total Cash Value")]
        public AccountComplexDoubleAttributeModel TotalCashValue { get; set; }

        [Display(Name = "Unrealized PnL")]
        public string UnrealizedPnL { get; set; }
    }

    public class AccountComplexDoubleAttributeModel
    {
        [Display(Name = "Commodities")]
        public string CommoditiesValue { get; set; }

        [Display(Name = "Stocks")]
        public string StocksValue { get; set; }

        [Display(Name = "Total")]
        public string TotalValue { get; set; }
    }

    internal static class AccountModelExtensions
    {
        private static AccountComplexDoubleAttributeModel ToAccountComplexDoubleAttributeModel(this AccountComplexDoubleAttribute attribute)
        {
            if (attribute == null)
                return new AccountComplexDoubleAttributeModel();

            string ccy = attribute.Currency != Currency.UNKNOWN ? attribute.Currency.ToString() : "";

            return new AccountComplexDoubleAttributeModel()
            {
                CommoditiesValue = $"{attribute.CommoditiesValue:N2} {ccy}",
                StocksValue = $"{attribute.StocksValue:N2} {ccy}",
                TotalValue = $"{attribute.TotalValue:N2} {ccy}"
            };
        }

        private static string FormatAccountLedgerAttribute(AccountLedgerAttribute attribute, Currency baseCurrency)
        {
            if (attribute == null)
                return null;

            if (baseCurrency == Currency.UNKNOWN || baseCurrency == Currency.USD)
                return $"{attribute.UsdValue:N2} USD";
            else
                return $"{attribute.BaseCurrencyValue:N2} {baseCurrency} ({attribute.UsdValue:N2} USD)";
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
                CashBalance = FormatAccountLedgerAttribute(account.CashBalance, account.BaseCurrency),
                FullAvailableFunds = account.FullAvailableFunds.ToAccountComplexDoubleAttributeModel(),
                FullExcessLiquidity = account.FullExcessLiquidity.ToAccountComplexDoubleAttributeModel(),
                FullInitMarginReq = account.FullInitMarginReq.ToAccountComplexDoubleAttributeModel(),
                FullMaintMarginReq = account.FullMaintMarginReq.ToAccountComplexDoubleAttributeModel(),
                Leverage = account.Leverage.ToAccountComplexDoubleAttributeModel(),
                LastUpdate = account.LastUpdate,
                Name = account.Name,
                RealizedPnL = FormatAccountLedgerAttribute(account.RealizedPnL, account.BaseCurrency),
                TotalCashBalance = FormatAccountLedgerAttribute(account.TotalCashBalance, account.BaseCurrency),
                TotalCashValue = account.TotalCashValue.ToAccountComplexDoubleAttributeModel(),
                UnrealizedPnL = FormatAccountLedgerAttribute(account.UnrealizedPnL, account.BaseCurrency)
            };
        }

        public static List<AccountModel> ToAccountModels(this IEnumerable<Account> accounts)
        {
            return accounts?.Select(a => a.ToAccountModel()).ToList();
        }
    }
}
