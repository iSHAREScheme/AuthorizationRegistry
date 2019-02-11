using Newtonsoft.Json;

namespace iSHARE.AuthorizationRegistry.Api.ViewModels
{
    public class DelegationTokenResponse
    {
        [JsonProperty("delegation_token")]
        public string DelegationToken { get; set; }
    }
}
