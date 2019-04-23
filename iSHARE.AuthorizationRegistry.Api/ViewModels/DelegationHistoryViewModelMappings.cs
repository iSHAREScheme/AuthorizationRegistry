using iSHARE.AuthorizationRegistry.Core.Models;

namespace iSHARE.AuthorizationRegistry.Api.ViewModels
{
    public static class DelegationHistoryViewModelMappings
    {
        public static DelegationHistoryViewModel MapToViewModel(this DelegationHistory delegationHistory)
        {
            return new DelegationHistoryViewModel
            {
                Id = delegationHistory.Id,
                CreatedDate = delegationHistory.CreatedDate,
                CreatedBy = delegationHistory.CreatedBy?.Name,
                Policy = delegationHistory.Policy
            };
        }
    }
}
