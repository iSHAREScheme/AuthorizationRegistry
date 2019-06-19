using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using IdentityModel.Client;
using iSHARE.Configuration.Configurations;
using iSHARE.Identity.Responses;
using iSHARE.Models;
using Microsoft.AspNetCore.Http;
using Response = iSHARE.Models.Response;

namespace iSHARE.Identity.Login
{
    public class SessionHandleService : ISessionHandleService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _httpClient;
        private readonly SchemeOwnerIdentityProviderOptions _idpOptions;
        private readonly SpaOptions _spaOptions;
        private readonly PartyDetailsOptions _partyDetailsOptions;

        public SessionHandleService(IHttpContextAccessor httpContext,
            HttpClient httpClient,
            SchemeOwnerIdentityProviderOptions idpOptions,
            SpaOptions spaOptions,
            PartyDetailsOptions partyDetailsOptions)
        {
            _httpContext = httpContext;
            _httpClient = httpClient;
            _idpOptions = idpOptions;
            _spaOptions = spaOptions;
            _partyDetailsOptions = partyDetailsOptions;
        }
        public async Task<Response> Logout()
        {
            string token = _httpContext.HttpContext.Request.Headers["Authorization"];
            var authority = _idpOptions.Enable ? _idpOptions.AuthorityUrl : _partyDetailsOptions.BaseUri;
            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                var result = await _httpClient.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = Path.Combine(authority, "connect/revocation"),
                    ClientId = _spaOptions.SpaClientId,
                    ClientSecret = _spaOptions.SpaClientSecret,
                    Token = token.Substring("Bearer ".Length).Trim()
                });
            }

            return Response.ForSuccess();
        }

        public async Task<Response<AuthorizationCodeTokenResponse>> GetAccessToken(Requests.AuthorizationCodeTokenRequest request)
        {
            var authority = _idpOptions.Enable ? _idpOptions.AuthorityUrl : _partyDetailsOptions.BaseUri;

            using (var requestBody = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"scope", "iSHARE"},
                {"client_id", _spaOptions.SpaClientId},
                {"client_secret", _spaOptions.SpaClientSecret},
                {"redirect_uri", Path.Combine(_spaOptions.BaseUri, "callback")},
                {"code",request.Code}
            }))
            {
                var tokenResponse = await authority.AppendPathSegment("connect/token")
                        .PostAsync(requestBody)

                    ;
                if (tokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    var content = await Task.FromResult(tokenResponse).ReceiveJson();
                    return Response<AuthorizationCodeTokenResponse>.ForSuccess(
                        new AuthorizationCodeTokenResponse
                        {
                            AccessToken = (string)content.access_token,
                            ExpiresIn = (int)content.expires_in
                        });
                }

            }
            return Response.ForError("Token retrieval failed");
        }
    }
}
