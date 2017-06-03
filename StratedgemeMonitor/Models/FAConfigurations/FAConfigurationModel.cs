using Capital.GSG.FX.Data.Core.FinancialAdvisorsData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StratedgemeMonitor.Models.FAConfigurations
{
    public class FAConfigurationModel
    {
        public string MasterAccount { get; set; }

        [Display(Name = "Account Aliases")]
        public List<FAAccountAliasModel> AccountAliases { get; set; }
        public List<FAGroupModel> Groups { get; set; }

        [Display(Name = "Allocation Profiles")]
        public List<FAAllocationProfileModel> AllocationProfiles { get; set; }


        [Display(Name = "Last Updated")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss zzz}")]
        public DateTimeOffset LastUpdated { get; set; }
    }

    public class FAAccountAliasModel
    {
        public string Account { get; set; }
        public string Alias { get; set; }
    }

    public class FAGroupModel
    {
        public string Name { get; set; }
        public List<string> Accounts { get; set; }

        [Display(Name = "Default Method")]
        public FAGroupMethod DefaultMethod { get; set; }
    }

    public class FAAllocationProfileModel
    {
        public string Name { get; set; }
        public FAAllocationProfileType Type { get; set; }
        public List<FAAllocationModel> Allocations { get; set; }
    }

    public class FAAllocationModel
    {
        public string Account { get; set; }

        [DisplayFormat(DataFormatString = "{0:N1}")]
        public double Amount { get; set; }
    }

    internal static class FAConfigurationModelExtensions
    {
        private static FAAccountAliasModel ToFAAccountAliasModel(this FAAccountAlias alias)
        {
            if (alias == null)
                return null;

            return new FAAccountAliasModel()
            {
                Account = alias.Account,
                Alias = alias.Alias
            };
        }

        private static List<FAAccountAliasModel> ToFAAccountAliasModels(this IEnumerable<FAAccountAlias> aliases)
        {
            return aliases?.Select(a => a.ToFAAccountAliasModel()).ToList() ?? new List<FAAccountAliasModel>();
        }

        private static FAAllocationModel ToFAAllocationModel(this FAAllocation allocation)
        {
            if (allocation == null)
                return null;

            return new FAAllocationModel()
            {
                Account = allocation.Account,
                Amount = allocation.Amount
            };
        }

        private static List<FAAllocationModel> ToFAAllocationModels(this IEnumerable<FAAllocation> allocations)
        {
            return allocations?.Select(a => a.ToFAAllocationModel()).ToList() ?? new List<FAAllocationModel>();
        }

        private static FAAllocationProfileModel ToFAAllocationProfileModel(this FAAllocationProfile profile)
        {
            if (profile == null)
                return null;

            return new FAAllocationProfileModel()
            {
                Allocations = profile.Allocations.ToFAAllocationModels(),
                Name = profile.Name,
                Type = profile.Type
            };
        }

        private static List<FAAllocationProfileModel> ToFAAllocationProfileModels(this IEnumerable<FAAllocationProfile> profiles)
        {
            return profiles?.Select(p => p.ToFAAllocationProfileModel()).ToList();
        }

        private static FAGroupModel ToFAGroupModel(this FAGroup group)
        {
            if (group == null)
                return null;

            return new FAGroupModel()
            {
                Accounts = group.Accounts,
                DefaultMethod = group.DefaultMethod,
                Name = group.Name
            };
        }

        private static List<FAGroupModel> ToFAGroupModels(this IEnumerable<FAGroup> groups)
        {
            return groups?.Select(g => g.ToFAGroupModel()).ToList();
        }

        public static FAConfigurationModel ToFAConfigurationModel(this FAConfiguration configuration)
        {
            if (configuration == null)
                return null;

            return new FAConfigurationModel()
            {
                AccountAliases = configuration.AccountAliases.ToFAAccountAliasModels(),
                AllocationProfiles = configuration.AllocationProfiles.ToFAAllocationProfileModels(),
                Groups = configuration.Groups.ToFAGroupModels(),
                LastUpdated = configuration.LastUpdated.ToLocalTime(),
                MasterAccount = configuration.MasterAccount
            };
        }

        public static List<FAConfigurationModel> ToFAConfigurationModels(this IEnumerable<FAConfiguration> configurations)
        {
            return configurations?.Select(c => c.ToFAConfigurationModel()).ToList();
        }

        private static FAAccountAlias ToFAAccountAlias(this FAAccountAliasModel alias)
        {
            if (alias == null)
                return null;

            return new FAAccountAlias()
            {
                Account = alias.Account,
                Alias = alias.Alias
            };
        }

        private static List<FAAccountAlias> ToFAAccountAliases(this IEnumerable<FAAccountAliasModel> aliases)
        {
            return aliases?.Select(a => a.ToFAAccountAlias()).ToList();
        }

        private static FAAllocation ToFAAllocation(this FAAllocationModel allocation)
        {
            if (allocation == null)
                return null;

            return new FAAllocation()
            {
                Account = allocation.Account,
                Amount = allocation.Amount
            };
        }

        private static List<FAAllocation> ToFAAllocations(this IEnumerable<FAAllocationModel> allocations)
        {
            return allocations?.Select(a => a.ToFAAllocation()).ToList();
        }

        private static FAAllocationProfile ToFAAllocationProfile(this FAAllocationProfileModel profile)
        {
            if (profile == null)
                return null;

            return new FAAllocationProfile()
            {
                Allocations = profile.Allocations.ToFAAllocations(),
                Name = profile.Name,
                Type = profile.Type
            };
        }

        private static List<FAAllocationProfile> ToFAAllocationProfiles(this IEnumerable<FAAllocationProfileModel> profiles)
        {
            return profiles?.Select(p => p.ToFAAllocationProfile()).ToList();
        }

        private static FAGroup ToFAGroup(this FAGroupModel group)
        {
            if (group == null)
                return null;

            return new FAGroup()
            {
                Accounts = group.Accounts,
                DefaultMethod = group.DefaultMethod,
                Name = group.Name
            };
        }

        private static List<FAGroup> ToFAGroups(this IEnumerable<FAGroupModel> groups)
        {
            return groups?.Select(g => g.ToFAGroup()).ToList();
        }

        public static FAConfiguration ToFAConfiguration(this FAConfigurationModel configuration)
        {
            if (configuration == null)
                return null;

            return new FAConfiguration()
            {
                AccountAliases = configuration.AccountAliases.ToFAAccountAliases(),
                AllocationProfiles = configuration.AllocationProfiles.ToFAAllocationProfiles(),
                Groups = configuration.Groups.ToFAGroups(),
                LastUpdated = configuration.LastUpdated,
                MasterAccount = configuration.MasterAccount
            }; ;
        }

        public static List<FAConfiguration> ToFAConfigurations(this IEnumerable<FAConfigurationModel> configurations)
        {
            return configurations?.Select(c => c.ToFAConfiguration()).ToList();
        }
    }
}
