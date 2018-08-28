using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLIP.iShare.IdentityServer;
using NLIP.iShare.IdentityServer.Models;
using NLIP.iShare.IdentityServer.Validation;
using NLIP.iShare.TokenClient;
using System.Linq;
using System.Threading.Tasks;
using ClientAssertion = NLIP.iShare.IdentityServer.ClientAssertion;

namespace NLIP.iShare.SchemeOwner.Client
{
    public class SchemeOwnerClient : ISchemeOwnerClient
    {
        private readonly ILogger _logger;
        private readonly ITokenClient _tokenClient;

        private string _schemeOwnerBaseUrl;
        private string _privateKey;
        private string[] _publicKeys;
        private string _clientId;

        public SchemeOwnerClient(ITokenClient tokenClient, IConfiguration configuration, ILogger<SchemeOwnerClient> logger)
        {
            _logger = logger;
            _tokenClient = tokenClient;

            _schemeOwnerBaseUrl = configuration["SchemeOwner:BaseUri"];
            _privateKey = configuration["MyDetails:PrivateKey"];
            _publicKeys = configuration.GetSection("MyDetails:PublicKeys").Get<string[]>();
            _clientId = configuration["MyDetails:ClientId"];

            if (string.IsNullOrEmpty(_schemeOwnerBaseUrl))
            {
                throw new SchemeOwnerClientException("The SchemeOwner Uri is not configured.");
            }

            if (string.IsNullOrEmpty(_privateKey))
            {
                throw new SchemeOwnerClientException("The PrivateKey is not configured.");
            }

            if (string.IsNullOrEmpty(_clientId))
            {
                throw new SchemeOwnerClientException("The ClientId is not configured.");
            }

            if (!_publicKeys.Any())
            {
                throw new SchemeOwnerClientException("The Public Keys are not configured.");
            }
        }

        public async Task<CertificateStatus> GetCertificate(string clientAssertion, string certificateHash)
        {
            var accessToken = await _tokenClient
                .GetAccessToken(_schemeOwnerBaseUrl, _clientId, clientAssertion)
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

            var certificateStatus = await _schemeOwnerBaseUrl
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

            var status = await _schemeOwnerBaseUrl
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
            var authorityAudience = $"{_schemeOwnerBaseUrl.TrimEnd('/')}/connect/token";

            var assertion = new TokenClient.ClientAssertion
            {
                Subject = _clientId,
                Issuer = _clientId,
                Audience = _clientId,
                AuthorityAudience = authorityAudience,
                JwtId = clientAssertion.JwtId,
                IssuedAt = clientAssertion.IssuedAt,
                Expiration = clientAssertion.Expiration
            };

            var accessToken = await _tokenClient
                .GetAccessToken(_schemeOwnerBaseUrl, _clientId, assertion, _privateKey, _publicKeys)
                .ConfigureAwait(false);
            return accessToken;
        }
    }
}
