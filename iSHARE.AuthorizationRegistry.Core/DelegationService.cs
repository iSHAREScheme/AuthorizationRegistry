using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Core.Models;
using iSHARE.AuthorizationRegistry.Core.Requests;
using iSHARE.IdentityServer.Delegation;
using iSHARE.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace iSHARE.AuthorizationRegistry.Core
{
    internal class DelegationService : IDelegationService
    {
        private readonly IDelegationsRepository _delegationsRepository;
        private readonly IUsersRepository _usersRepository;

        public DelegationService(IDelegationsRepository delegationsRepository, IUsersRepository usersRepository)
        {
            _delegationsRepository = delegationsRepository;
            _usersRepository = usersRepository;
        }

        public async Task<PagedResult<Delegation>> GetAll(DelegationQuery query)
        {
            return await _delegationsRepository.GetAll(query);
        }

        public async Task<Delegation> GetByPolicyId(string policyId, string partyId)
        {
            var delegation = await _delegationsRepository.GetByPolicyId(policyId, partyId);
            if (delegation != null)
            {
                delegation.DelegationHistory = (await _delegationsRepository.GetHistory(policyId, partyId)).ToList();
            }

            return delegation;
        }

        public async Task<Delegation> GetBySubject(string subject, string partyId)
        {
            return await _delegationsRepository.GetBySubject(subject, partyId);
        }

        public async Task<Delegation> Get(Guid id, string partyId)
        {
            return await _delegationsRepository.Get(id, partyId);
        }

        public async Task<Delegation> Copy(CreateOrUpdateDelegationRequest request)
        {
            var policyJson = new DelegationPolicyJsonParser(request.Policy);
            var newPolicyIssuer = policyJson.PolicyIssuer;
            var newAccessSubject = policyJson.AccessSubject;

            if (!await DelegationExists(newPolicyIssuer, newAccessSubject))
            {
                return await Create(request);
            }

            var policyId = await _delegationsRepository.GetPolicyId(newPolicyIssuer, newAccessSubject);

            return await Update(policyId, request);
        }

        public async Task<Delegation> Create(CreateOrUpdateDelegationRequest request)
        {
            var policyJson = new DelegationPolicyJsonParser(request.Policy);
            var policyIssuer = policyJson.PolicyIssuer;
            var accessSubject = policyJson.AccessSubject;

            var createdBy = await _usersRepository.GetByIdentity(request.UserId);

            var delegation = new Delegation
            {
                AuthorizationRegistryId = await GenerateId(),
                PolicyIssuer = policyIssuer,
                AccessSubject = accessSubject,
                Policy = request.Policy,
                CreatedById = createdBy.Id,
                UpdatedById = createdBy.Id,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                DelegationHistory = new List<DelegationHistory>()
            };

            _delegationsRepository.Add(delegation);

            await _delegationsRepository.Save();
            return delegation;
        }

        public async Task<Delegation> Update(string policyId, CreateOrUpdateDelegationRequest request)
        {
            var entity = await GetByPolicyId(policyId, request.PartyId);
            var createdBy = await _usersRepository.GetByIdentity(request.UserId);

            var policyJson = new DelegationPolicyJsonParser(request.Policy);
            var policyIssuer = policyJson.PolicyIssuer;
            var accessSubject = policyJson.AccessSubject;

            entity.DelegationHistory.Add(new DelegationHistory
            {
                DelegationId = entity.Id,
                Policy = entity.Policy,
                CreatedDate = DateTime.Now,
                CreatedById = createdBy.Id
            });

            entity.PolicyIssuer = policyIssuer;
            entity.AccessSubject = accessSubject;
            entity.Policy = request.Policy;
            entity.UpdatedById = createdBy.Id;
            entity.UpdatedDate = DateTime.Now;

            await _delegationsRepository.Save();

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
                delegation = UpdateExistingDelegation(delegation, policyJson);
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

                _delegationsRepository.Add(delegation);
            }

            await _delegationsRepository.Save();
            return delegation;
        }


        public async Task MakeInactive(string policyId, string userId)
        {
            var currentUser = await _usersRepository.GetByIdentity(userId);

            var delegation = await _delegationsRepository.GetByPolicyId(policyId);

            delegation.Deleted = true;
            delegation.DelegationHistory.Add(new DelegationHistory
            {
                CreatedById = currentUser.Id,
                DelegationId = delegation.Id,
                Policy = delegation.Policy,
                CreatedDate = DateTime.Now
            });

            await _delegationsRepository.Save();
        }

        public async Task<bool> DelegationExists(string partyId, string subject)
        {
            return await _delegationsRepository.Exists(partyId, subject);

        }

        public async Task<DelegationHistory> GetDelegationHistoryById(DelegationHistoryQuery query)
        {
            return await _delegationsRepository.GetHistory(query);
        }

        private async Task<string> GenerateId()
        {
            const string prefix = "AR";
            var id = FriendlyIdGenerator.New(prefix);

            while (await _delegationsRepository.Exists(id))
            {
                id = FriendlyIdGenerator.New(prefix);
            }

            return id;
        }

        private Delegation UpdateExistingDelegation(Delegation delegation, string newPolicy)
        {
            delegation.DelegationHistory.Add(new DelegationHistory
            {
                DelegationId = delegation.Id,
                Policy = delegation.Policy,
                CreatedDate = DateTime.Now,
            });

            delegation.Policy = newPolicy;
            delegation.UpdatedDate = DateTime.Now;
            return delegation;
        }

        private async Task<Delegation> GetExistingDelegation(string policyIssuer, string accessSubject)
        {
            return await _delegationsRepository.GetByPolicyIssuerAndAccessSubject(policyIssuer, accessSubject);
        }

    }
}
