using NLIP.iShare.Abstractions;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using System;
using System.Threading.Tasks;

namespace NLIP.iShare.AuthorizationRegistry.Core.Api
{
    /// <summary>
    /// Defines the use cases related to delegation management
    /// </summary>
    public interface IDelegationService
    {
        Task<PagedResult<Delegation>> GetAll(DelegationQuery query);
        Task<Delegation> Get(Guid id, string partyId);
        Task<Delegation> GetByArId(string arId, string partyId);
        Task<Delegation> GetBySubject(string subject, string partyId);
        Task<Delegation> Copy(CreateOrUpdateDelegationRequest request);
        Task<Delegation> Create(CreateOrUpdateDelegationRequest request);
        Task<Delegation> Update(string arId, CreateOrUpdateDelegationRequest request);
        Task MakeInactive(string arId, string userId);
        Task<bool> DelegationExists(string partyId, string subject);        
        Task<DelegationHistory> GetDelegationHistoryById(DelegationHistoryQuery query);
        Task<Delegation> CreateOrUpdatePolicyForParty(PolicyRequest policy, string partyId);
    }
}
