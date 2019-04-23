using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Core.Models;
using iSHARE.AuthorizationRegistry.Core.Requests;
using iSHARE.EntityFramework;
using iSHARE.Models;
using Microsoft.EntityFrameworkCore;

namespace iSHARE.AuthorizationRegistry.Data
{
    public class DelegationsRepository : IDelegationsRepository
    {
        private readonly AuthorizationRegistryDbContext _db;

        public DelegationsRepository(AuthorizationRegistryDbContext db)
        {
            _db = db;
        }


        public async Task<bool> Exists(string id)
        {
            return await _db.Delegations
                    .AnyAsync(d => d.AuthorizationRegistryId == id && !d.Deleted);
        }

        public async Task<bool> Exists(string partyId, string subject)
        {
            return await _db.Delegations
                    .AnyAsync(d => d.PolicyIssuer == partyId && d.AccessSubject == subject && !d.Deleted)
                ;
        }

        public async Task<DelegationHistory> GetHistory(DelegationHistoryQuery query)
        {
            var history = await _db.DelegationsHistories
                    .Include(d => d.Delegation)
                    .FirstOrDefaultAsync(dh => dh.Id == query.Id
                                               && (query.PartyId == Constants.SchemeOwnerPartyId || dh.Delegation.PolicyIssuer == query.PartyId))
                ;

            return history;
        }

        public async Task<IReadOnlyCollection<DelegationHistory>> GetHistory(string arId, string partyId)
        {
            var history = await _db.DelegationsHistories
                .Include(dh => dh.CreatedBy)
                .Where(dh => !dh.Delegation.Deleted && dh.Delegation.AuthorizationRegistryId == arId
                                                    && (partyId == Constants.SchemeOwnerPartyId || dh.Delegation.PolicyIssuer == partyId))
                .ToListAsync();

            return history;
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

            var count = await delegations.CountAsync();

            delegations = Sort(query, delegations).GetPage(query);

            return await delegations.ToPagedResult(count);
        }

        public void Add(Delegation delegation)
        {
            _db.Delegations.Add(delegation);
        }

        public async Task<Delegation> GetByPolicyId(string arId, string partyId)
        {
            var delegation = await _db.Delegations
                    .Include(d => d.CreatedBy)
                    .FirstOrDefaultAsync(d => d.AuthorizationRegistryId == arId
                                              && (partyId == Constants.SchemeOwnerPartyId || d.PolicyIssuer == partyId)
                                              && !d.Deleted)
                ;
            return delegation;
        }

        public async Task<Delegation> GetByPolicyId(string arId)
        {
            var delegation = await _db.Delegations
                    .Include(d => d.CreatedBy)
                    .FirstOrDefaultAsync(d => d.AuthorizationRegistryId == arId)
                ;
            return delegation;
        }

        public async Task<Delegation> GetBySubject(string subject, string partyId)
        {
            var delegation = await _db.Delegations
                    .Include(d => d.CreatedBy)
                    .FirstOrDefaultAsync(d => d.AccessSubject == subject
                                              && (partyId == Constants.SchemeOwnerPartyId || d.PolicyIssuer == partyId)
                                              && !d.Deleted)
                ;

            return delegation;
        }

        public async Task<Delegation> Get(Guid id, string partyId)
        {
            var delegation = await _db.Delegations
                    .FirstOrDefaultAsync(d => d.Id == id && (partyId == Constants.SchemeOwnerPartyId || d.PolicyIssuer == partyId) && !d.Deleted)
                ;

            return delegation;
        }

        public async Task<string> GetPolicyId(string newPolicyIssuer, string newAccessSubject)
        {
            var policyId = await _db.Delegations
                    .Where(d => d.PolicyIssuer == newPolicyIssuer && d.AccessSubject == newAccessSubject)
                    .Select(d => d.AuthorizationRegistryId)
                    .FirstAsync()
                ;
            return policyId;
        }

        public async Task<Delegation> GetByPolicyIssuerAndAccessSubject(string policyIssuer, string accessSubject)
        {
            return await _db.Delegations
                .FirstOrDefaultAsync(d => d.PolicyIssuer == policyIssuer && d.AccessSubject == accessSubject && !d.Deleted);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
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
    }
}
