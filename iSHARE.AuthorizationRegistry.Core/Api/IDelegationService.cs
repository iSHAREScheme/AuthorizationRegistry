﻿using System;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Models;
using iSHARE.AuthorizationRegistry.Core.Requests;

namespace iSHARE.AuthorizationRegistry.Core.Api
{
    /// <summary>
    /// Defines the use cases related to delegation management
    /// </summary>
    public interface IDelegationService
    {
        Task<PagedResult<Delegation>> GetAll(DelegationQuery query);
        Task<Delegation> Get(Guid id, string partyId);
        Task<Delegation> GetByPolicyId(string policyId, string partyId);
        Task<Delegation> GetBySubject(string subject, string partyId);
        Task<Delegation> Copy(CreateOrUpdateDelegationRequest request);
        Task<Delegation> Create(CreateOrUpdateDelegationRequest request);
        Task<Delegation> Update(string policyId, CreateOrUpdateDelegationRequest request);
        Task MakeInactive(string policyId, string userId);
        Task<bool> DelegationExists(string partyId, string subject);
        Task<DelegationHistory> GetDelegationHistoryById(DelegationHistoryQuery query);
        Task<Delegation> CreateOrUpdatePolicyForParty(PolicyRequest policy, string partyId);
    }
}
