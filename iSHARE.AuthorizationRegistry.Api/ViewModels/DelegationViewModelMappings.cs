using System.Linq;
using iSHARE.AuthorizationRegistry.Core.Models;

namespace iSHARE.AuthorizationRegistry.Api.ViewModels
{
    public static class DelegationViewModelMappings
    {
        public static DelegationViewModel MapToViewModel(this Delegation delegation)
        {
            return new DelegationViewModel
            {
                Id = delegation.Id,
                AuthorizationRegistryId = delegation.AuthorizationRegistryId,
                Policy = delegation.Policy,
                CreatedDate = delegation.CreatedDate,
                CreatedBy = delegation.CreatedBy?.Name,
                History = delegation.DelegationHistory.Select(d => d.MapToViewModel())
            };
        }
        public static DelegationOverviewViewModel MapToOverviewViewModel(this Delegation delegation)
        {
            return new DelegationOverviewViewModel
            {
                Id = delegation.Id,
                AuthorizationRegistryId = delegation.AuthorizationRegistryId,
                PolicyIssuer = delegation.PolicyIssuer,
                Subject = delegation.AccessSubject
            };
        }
    }
}
