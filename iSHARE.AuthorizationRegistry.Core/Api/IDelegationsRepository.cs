using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Models;
using iSHARE.AuthorizationRegistry.Core.Requests;

namespace iSHARE.AuthorizationRegistry.Core.Api
{
    public interface IDelegationsRepository
    {
        Task<bool> Exists(string id);
        Task<bool> Exists(string partyId, string subject);
        Task<DelegationHistory> GetHistory(DelegationHistoryQuery query);
        Task<IReadOnlyCollection<DelegationHistory>> GetHistory(string arId, string partyId);
        Task<PagedResult<Delegation>> GetAll(DelegationQuery query);
        void Add(Delegation delegation);
        Task<Delegation> GetByPolicyId(string policyId, string partyId);
        Task<Delegation> GetByPolicyId(string policyId);
        Task<Delegation> GetBySubject(string subject, string partyId);
        Task<Delegation> Get(Guid id, string partyId);
        Task<string> GetPolicyId(string newPolicyIssuer, string newAccessSubject);
        Task<Delegation> GetByPolicyIssuerAndAccessSubject(string policyIssuer, string accessSubject);
        Task Save();
    }
}
