using NLIP.iShare.AuthorizationRegistry.Data.Models;

namespace NLIP.iShare.AuthorizationRegistry.Api.ViewModels
{
    public static class DelegationHistoryViewModelMappings
    {
        public static DelegationHistoryViewModel MapToViewModel(this DelegationHistory delegationHistory)
        {
            return new DelegationHistoryViewModel()
            {
                Id = delegationHistory.Id,
                CreatedDate = delegationHistory.CreatedDate,
                CreatedBy = delegationHistory.CreatedBy.Name,
                Policy = delegationHistory.Policy
            };
        }
    }
}