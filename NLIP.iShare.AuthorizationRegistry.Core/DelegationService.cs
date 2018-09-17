using Microsoft.EntityFrameworkCore;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.AuthorizationRegistry.Data;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLIP.iShare.Abstractions;
using NLIP.iShare.EntityFramework;


namespace NLIP.iShare.AuthorizationRegistry.Core
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
                .Where(d => !d.Deleted && (query.PartyId == null || d.PolicyIssuer == query.PartyId));

            int count = await delegations.CountAsync().ConfigureAwait(false);

            delegations = delegations
                .OrderByDescending(d => d.CreatedDate)
                .GetPage(query);

            return await delegations.ToPagedResult(count).ConfigureAwait(false);
        }

        public async Task<Delegation> GetByArId(string arId, string partyId)
        {
            var delegation = await _db.Delegations
                .Include(d => d.CreatedBy)
                .FirstOrDefaultAsync(d => d.AuthorizationRegistryId == arId
                                     && (partyId == null || d.PolicyIssuer == partyId)
                                     && !d.Deleted)
                .ConfigureAwait(false);

            delegation.DelegationHistory = await GetDelegationHistoryByDelegationARId(arId, partyId).ConfigureAwait(false);

            return delegation;
        }

        public async Task<Delegation> GetBySubject(string subject, string partyId)
        {
            var delegation = await _db.Delegations
                .Include(d => d.CreatedBy)
                .FirstOrDefaultAsync(d => d.AccessSubject == subject
                                     && (partyId == null || d.PolicyIssuer == partyId)
                                     && !d.Deleted)
                .ConfigureAwait(false);

            return delegation;
        }

        public async Task<Delegation> Get(Guid id, string partyId)
        {
            var delegation = await _db.Delegations
                .FirstOrDefaultAsync(d => d.Id == id && (partyId == null || d.PolicyIssuer == partyId) && !d.Deleted)
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

            var delegation = new Delegation()
            {
                AuthorizationRegistryId = await GenerateArId().ConfigureAwait(false),
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
                             && (partyId == null || dh.Delegation.PolicyIssuer == partyId))
                .ToListAsync()
                .ConfigureAwait(false);

            return history;
        }

        public async Task<DelegationHistory> GetDelegationHistoryById(DelegationHistoryQuery query)
        {
            var history = await _db.DelegationsHistories
                .Include(d => d.Delegation)
                .FirstOrDefaultAsync(dh => dh.Id == query.Id
                                     && (query.PartyId == null || dh.Delegation.PolicyIssuer == query.PartyId))
                .ConfigureAwait(false);

            return history;
        }

        private async Task<string> GenerateArId()
        {
            var arId = AuthorizationRegistryIdGenerator.New();

            if (await _db.Delegations.AnyAsync(d => d.AuthorizationRegistryId == arId && !d.Deleted).ConfigureAwait(false))
            {
                return AuthorizationRegistryIdGenerator.New();
            }

            return arId;
        }
    }
}
