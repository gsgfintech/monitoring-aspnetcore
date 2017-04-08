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

        public List<AccountAttributeModel> Attributes { get; set; }
    }

    public class AccountAttributeModel
    {
        public Currency Currency { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }

    internal static class AccountModelExtensions
    {
        public static AccountModel ToAccountModel(this Account account)
        {
            if (account == null)
                return null;

            return new AccountModel()
            {
                Attributes = account.Attributes.ToAccountAttributeModels(),
                Broker = account.Broker,
                LastUpdate = account.LastUpdate,
                Name = account.Name
            };
        }

        public static List<AccountModel> ToAccountModels(this IEnumerable<Account> accounts)
        {
            return accounts?.Select(a => a.ToAccountModel()).ToList();
        }

        private static AccountAttributeModel ToAccountAttributeModel(this AccountAttribute attribute)
        {
            if (attribute == null)
                return null;

            return new AccountAttributeModel()
            {
                Currency = attribute.Currency,
                Key = attribute.Key,
                Value = attribute.Value
            };
        }

        public static List<AccountAttributeModel> ToAccountAttributeModels(this IEnumerable<AccountAttribute> attributes)
        {
            return attributes?.Select(a => a.ToAccountAttributeModel()).ToList();
        }
    }
}
