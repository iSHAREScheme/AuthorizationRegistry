using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Core.Requests;
using iSHARE.AuthorizationRegistry.Data;
using iSHARE.AuthorizationRegistry.Data.Models;
using iSHARE.EntityFramework;
using iSHARE.IdentityServer.Delegation;
using iSHARE.Models;
using Delegation = iSHARE.AuthorizationRegistry.Data.Models.Delegation;

namespace iSHARE.AuthorizationRegistry.Core
{
    internal class DelegationService : IDelegationService
    {
        private readonly AuthorizationRegistryDbContext _db;

        public DelegationService(AuthorizationRegistryDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<Delegation>> GetAll(DelegationQuery query)
        {
            var delegations = _db.Delegations
                .Where(d => !d.Deleted && (query.PartyId == Constants.SchemeOwnerPartyId || d.PolicyIssuer == query.PartyId));

            if (!string.IsNullOrEmpty(query.Filter))
            {
                delegations = delegations.Where(d =>
                    d.AuthorizationRegistryId.Contains(query.Filter) ||
                    d.PolicyIssuer.Contains(query.Filter) ||
                    d.AccessSubject.Contains(query.Filter));
            }

            int count = await delegations.CountAsync().ConfigureAwait(false);

            delegations = Sort(query, delegations).GetPage(query);

            return await delegations.ToPagedResult(count).ConfigureAwait(false);
        }

        public async Task<Delegation> GetByArId(string arId, string partyId)
        {
            var delegation = await _db.Delegations
                .Include(d => d.CreatedBy)
                .FirstOrDefaultAsync(d => d.AuthorizationRegistryId == arId
                                     && (partyId == Constants.SchemeOwnerPartyId || d.PolicyIssuer == partyId)
                                     && !d.Deleted)
                .ConfigureAwait(false);
            if (delegation != null)
            {
                delegation.DelegationHistory = await GetDelegationHistoryByDelegationARId(arId, partyId).ConfigureAwait(false);
                return delegation;
            }

            return null;
        }

        public async Task<Delegation> GetBySubject(string subject, string partyId)
        {
            var delegation = await _db.Delegations
                .Include(d => d.CreatedBy)
                .FirstOrDefaultAsync(d => d.AccessSubject == subject
                                     && (partyId == Constants.SchemeOwnerPartyId || d.PolicyIssuer == partyId)
                                     && !d.Deleted)
                .ConfigureAwait(false);

            return delegation;
        }

        public async Task<Delegation> Get(Guid id, string partyId)
        {
            var delegation = await _db.Delegations
                .FirstOrDefaultAsync(d => d.Id == id && (partyId == Constants.SchemeOwnerPartyId || d.PolicyIssuer == partyId) && !d.Deleted)
                .ConfigureAwait(false);

            return delegation;
        }

        public async Task<Delegation> Copy(CreateOrUpdateDelegationRequest request)
        {
            var policyJson = new DelegationPolicyJsonParser(request.Policy);
            var newPolicyIssuer = policyJson.PolicyIssuer;
            var newAccessSubject = policyJson.AccessSubject;

            if (!await DelegationExists(newPolicyIssuer, newAccessSubject).ConfigureAwait(false))
            {
                return await Create(request).ConfigureAwait(false);
            }

            var authorizationRegistryId = await _db.Delegations
                            .Where(d => d.PolicyIssuer == newPolicyIssuer && d.AccessSubject == newAccessSubject)
                            .Select(d => d.AuthorizationRegistryId)
                            .FirstAsync()
                            .ConfigureAwait(false);

            return await Update(authorizationRegistryId, request).ConfigureAwait(false);
        }

        public async Task<Delegation> Create(CreateOrUpdateDelegationRequest request)
        {
            var policyJson = new DelegationPolicyJsonParser(request.Policy);
            var policyIssuer = policyJson.PolicyIssuer;
            var accessSubject = policyJson.AccessSubject;

            var createdBy = _db.Users.First(u => u.AspNetUserId == request.UserId);

            var delegation = new Delegation
            {
                AuthorizationRegistryId = await GenerateId().ConfigureAwait(false),
                PolicyIssuer = policyIssuer,
                AccessSubject = accessSubject,
                Policy = request.Policy,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                DelegationHistory = new List<DelegationHistory>()
            };
            await _db.Delegations.AddAsync(delegation).ConfigureAwait(false);
            await _db.SaveChangesAsync().ConfigureAwait(false);

            return delegation;
        }

        public async Task<Delegation> Update(string arId, CreateOrUpdateDelegationRequest request)
        {
            var entity = await GetByArId(arId, request.PartyId).ConfigureAwait(false);
            var createdBy = _db.Users.First(u => u.AspNetUserId == request.UserId);

            var policyJson = new DelegationPolicyJsonParser(request.Policy);
            var policyIssuer = policyJson.PolicyIssuer;
            var accessSubject = policyJson.AccessSubject;

            await _db.DelegationsHistories.AddAsync(new DelegationHistory
            {
                DelegationId = entity.Id,
                Policy = entity.Policy,
                CreatedDate = DateTime.Now,
                CreatedBy = createdBy
            }).ConfigureAwait(false);

            entity.PolicyIssuer = policyIssuer;
            entity.AccessSubject = accessSubject;
            entity.Policy = request.Policy;
            entity.UpdatedBy = createdBy;
            entity.UpdatedDate = DateTime.Now;

            _db.Delegations.Update(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }

        public async Task<Delegation> CreateOrUpdatePolicyForParty(PolicyRequest policy, string partyId)
        {
            var policyIssuer = policy.DelegationEvidence.PolicyIssuer;
            var accessSubject = policy.DelegationEvidence.Target.AccessSubject;

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var policyJson = JsonConvert.SerializeObject(policy, serializerSettings);

            var delegation = await GetExistingDelegation(policyIssuer, accessSubject);
            if (delegation != null)
            {
                delegation = await UpdateExistingDelegation(delegation, policyJson);
            }
            else
            {
                delegation = new Delegation
                {
                    AuthorizationRegistryId = await GenerateId(),
                    PolicyIssuer = policyIssuer,
                    AccessSubject = accessSubject,
                    Policy = policyJson,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    DelegationHistory = new List<DelegationHistory>()
                };

                await _db.Delegations.AddAsync(delegation);
            }
            await _db.SaveChangesAsync();
            return delegation;
        }

        private async Task<Delegation> UpdateExistingDelegation(Delegation delegation, string newPolicy)
        {
            await _db.DelegationsHistories.AddAsync(new DelegationHistory
            {
                DelegationId = delegation.Id,
                Policy = delegation.Policy,
                CreatedDate = DateTime.Now,
            }).ConfigureAwait(false);

            delegation.Policy = newPolicy;
            delegation.UpdatedDate = DateTime.Now;

            _db.Delegations.Update(delegation);
            return delegation;
        }

        private async Task<Delegation> GetExistingDelegation(string policyIssuer, string accessSubject)
        {
            return await _db.Delegations
                .FirstOrDefaultAsync(d => d.PolicyIssuer == policyIssuer && d.AccessSubject == accessSubject && !d.Deleted);
        }

        public async Task MakeInactive(string arId, string userId)
        {
            var delegation = await _db.Delegations
                .FirstAsync(d => d.AuthorizationRegistryId == arId)
                .ConfigureAwait(false);
            var currentUser = await _db.Users
                .FirstAsync(u => u.AspNetUserId == userId)
                .ConfigureAwait(false);

            delegation.Deleted = true;

            await _db.DelegationsHistories.AddAsync(new DelegationHistory
            {
                CreatedById = currentUser.Id,
                DelegationId = delegation.Id,
                Policy = delegation.Policy,
                CreatedDate = DateTime.Now
            }).ConfigureAwait(false);

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<bool> DelegationExists(string partyId, string subject)
        {
            return await _db.Delegations
                .AnyAsync(d => d.PolicyIssuer == partyId && d.AccessSubject == subject && !d.Deleted)
                .ConfigureAwait(false);
        }

        public async Task<List<DelegationHistory>> GetDelegationHistoryByDelegationARId(string arId, string partyId)
        {
            var history = await _db.DelegationsHistories
                .Include(dh => dh.CreatedBy)
                .Where(dh => !dh.Delegation.Deleted && dh.Delegation.AuthorizationRegistryId == arId
                             && (partyId == Constants.SchemeOwnerPartyId || dh.Delegation.PolicyIssuer == partyId))
                .ToListAsync()
                .ConfigureAwait(false);

            return history;
        }

        public async Task<DelegationHistory> GetDelegationHistoryById(DelegationHistoryQuery query)
        {
            var history = await _db.DelegationsHistories
                .Include(d => d.Delegation)
                .FirstOrDefaultAsync(dh => dh.Id == query.Id
                                     && (query.PartyId == Constants.SchemeOwnerPartyId || dh.Delegation.PolicyIssuer == query.PartyId))
                .ConfigureAwait(false);

            return history;
        }

        private static IQueryable<Delegation> Sort(DelegationQuery query, IQueryable<Delegation> data)
        {
            var result = data;

            switch (query.SortBy)
            {
                case "policyIssuer":
                    result = result.OrderBy(c => c.PolicyIssuer, query.SortOrder);
                    break;
                case "accessSubject":
                    result = result.OrderBy(c => c.AccessSubject, query.SortOrder);
                    break;
                default:
                    result = result.OrderBy(c => c.AuthorizationRegistryId, query.SortOrder);
                    break;
            }

            return result;
        }

        private async Task<string> GenerateId()
        {
            const string prefix = "AR";
            var id = FriendlyIdGenerator.New(prefix);

            while (await _db.Delegations
                .AnyAsync(d => d.AuthorizationRegistryId == id && !d.Deleted)
                .ConfigureAwait(false))
            {
                id = FriendlyIdGenerator.New(prefix);
            }

            return id;
        }
    }
}
