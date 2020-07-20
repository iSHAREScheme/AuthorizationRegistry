using System.Threading.Tasks;
using iSHARE.IdentityServer.Validation.Interfaces;
using iSHARE.SchemeOwner.Client;

namespace iSHARE.Api.Validation
{
    internal class PartiesValidation : IPartiesValidation
    {
        private readonly ISchemeOwnerClient _client;

        public PartiesValidation(ISchemeOwnerClient client)
        {
            _client = client;
        }

        public async Task<bool> IsValidParty(string issuer)
        {
            var party = await _client.GetParty(issuer);

            if (party != null)
            {
                return party.IsValid();
            }

            return false;
        }
    }
}
