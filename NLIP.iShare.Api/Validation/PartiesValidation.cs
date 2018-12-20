using System.Threading.Tasks;
using NLIP.iShare.IdentityServer.Validation.Interfaces;
using NLIP.iShare.SchemeOwner.Client;

namespace NLIP.iShare.Api.Validation
{
    internal class PartiesValidation : IPartiesValidation
    {
        private readonly SchemeOwnerClient _client;
        public PartiesValidation(SchemeOwnerClient client)
        {
            _client = client;
        }

        public async Task<bool> IsValidParty(string issuer)
        {
            var party = await _client.GetParty(issuer);

            if (party != null)
            {
                return party.IsValid;
            }

            return false;
        }
    }
}
