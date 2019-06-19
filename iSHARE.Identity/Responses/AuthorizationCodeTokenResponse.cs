using Newtonsoft.Json;

namespace iSHARE.Identity.Responses
{
    public class AuthorizationCodeTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
