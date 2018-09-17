using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using NLIP.iShare.IdentityServer;
using NLIP.iShare.IdentityServer.Models;
using NLIP.iShare.IdentityServer.Validation;
using NLIP.iShare.TokenClient;
using System.Threading.Tasks;
using NLIP.iShare.Configuration.Configurations;
using ClientAssertion = NLIP.iShare.IdentityServer.ClientAssertion;

namespace NLIP.iShare.SchemeOwner.Client
{
    public class SchemeOwnerClient : ISchemeOwnerClient
    {
        private readonly ILogger _logger;
        private readonly ITokenClient _tokenClient;

        private readonly SchemeOwnerClientOptions _schemeOwnerClientOptions;
        private readonly PartyDetailsOptions _partyDetailsOptions;

        public SchemeOwnerClient(ITokenClient tokenClient,
            SchemeOwnerClientOptions schemeOwnerClientOptions,
            PartyDetailsOptions partyDetailsOptions,
            ILogger<SchemeOwnerClient> logger)
        {
            _logger = logger;
            _tokenClient = tokenClient;
            _schemeOwnerClientOptions = schemeOwnerClientOptions;
            _partyDetailsOptions = partyDetailsOptions;
        }

        public async Task<CertificateStatus> GetCertificate(string clientAssertion, string certificateHash)
        {
            var accessToken = await _tokenClient
                .GetAccessToken(_schemeOwnerClientOptions.BaseUri, _partyDetailsOptions.ClientId, clientAssertion)
                .ConfigureAwait(false);

            return await GetCertificateStatus(accessToken, certificateHash).ConfigureAwait(false);
        }

        public async Task<CertificateStatus> GetCertificate(IdentityServer.ClientAssertion clientAssertion, string certificateHash)
        {
            var accessToken = await GetAccessToken(clientAssertion).ConfigureAwait(false);

            return await GetCertificateStatus(accessToken, certificateHash).ConfigureAwait(false);
        }

        private async Task<CertificateStatus> GetCertificateStatus(string accessToken, string certificateHash)
        {
            _logger.LogInformation("GetCertificateStatus: {accessToken}, {certificateHash}", accessToken, certificateHash);

            var certificateStatus = await _schemeOwnerClientOptions.BaseUri
                .AppendPathSegment("certificates")
                .AppendPathSegment(certificateHash)
                .SetQueryParam("full", true)
                .SetQueryParam("test", true)
                .WithOAuthBearerToken(accessToken)
                .GetAsync()
                .ReceiveJson<CertificateStatus>()
                .ConfigureAwait(false);

            _logger.LogInformation("Certificates status : {status}", certificateStatus?.IsCertified);

            return certificateStatus;
        }

        public async Task<CertificateValidationStatus> ValidateCertificate(IdentityServer.ClientAssertion clientAssertion, string[] chain)
        {
            _logger.LogInformation("ValidateCertificate: {certificatesCount}", chain?.Length);

            var accessToken = await GetAccessToken(clientAssertion).ConfigureAwait(false);

            var status = await _schemeOwnerClientOptions.BaseUri
               .AppendPathSegment("certificates/certificate_validation")
               .WithOAuthBearerToken(accessToken)
               .PostJsonAsync(chain)
               .ReceiveJson<CertificateValidationStatus>()
               .ConfigureAwait(false);

            _logger.LogInformation("CertificateValidationStatus : {status}", status?.Validity);

            if (status == null)
            {
                _logger.LogInformation("Scheme owner communication failed");
                status = new CertificateValidationStatus { Validity = false };
            }

            return status;
        }

        private async Task<string> GetAccessToken(ClientAssertion clientAssertion)
        {
            var authorityAudience = $"{_schemeOwnerClientOptions.BaseUri.TrimEnd('/')}/connect/token";

            var assertion = new TokenClient.ClientAssertion
            {
                Subject = _partyDetailsOptions.ClientId,
                Issuer = _partyDetailsOptions.ClientId,
                Audience = _partyDetailsOptions.ClientId,
                AuthorityAudience = authorityAudience,
                JwtId = clientAssertion.JwtId,
                IssuedAt = clientAssertion.IssuedAt,
                Expiration = clientAssertion.Expiration
            };

            var accessToken = await _tokenClient
                .GetAccessToken(_schemeOwnerClientOptions.BaseUri,
                    _partyDetailsOptions.ClientId, 
                    assertion,
                    _partyDetailsOptions.PrivateKey,
                    _partyDetailsOptions.PublicKeys)
                .ConfigureAwait(false);
            return accessToken;
        }
    }
}
