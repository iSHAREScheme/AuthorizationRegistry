using NLIP.iShare.Api.Responses;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLIP.iShare.Abstractions;

namespace NLIP.iShare.AuthorizationRegistry.Core.Api
{
    /// <summary>
    /// Defines the use cases related to delegation management
    /// </summary>
    public interface IDelegationService
    {
        Task<PagedResult<Delegation>> GetAll(DelegationQuery query);
        Task<Delegation> Get(Guid id, string partyId);
        Task<Delegation> GetByARId(string arId, string partyId);
        Task<Delegation> GetBySubject(string subject, string partyId);
        Task<Delegation> Copy(CreateOrUpdateDelegationRequest request);
        Task<Delegation> Create(CreateOrUpdateDelegationRequest request);
        Task<Delegation> Update(string arId, CreateOrUpdateDelegationRequest request);
        Task MakeInactive(string arId, string userId);
        Task<bool> DelegationExists(string partyId, string subject);
        Task<List<DelegationHistory>> GetDelegationHistoryByDelegationARId(string arId, string partyId);
        Task<DelegationHistory> GetDelegationHistoryById(DelegationHistoryQuery query);
    }
}
