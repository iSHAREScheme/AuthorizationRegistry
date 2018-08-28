using Newtonsoft.Json;

namespace NLIP.iShare.AuthorizationRegistry.Api.ViewModels
{
    public class DelegationTokenResponse
    {
        [JsonProperty("delegation_token")]
        public string DelegationToken { get; set; }
    }
}
