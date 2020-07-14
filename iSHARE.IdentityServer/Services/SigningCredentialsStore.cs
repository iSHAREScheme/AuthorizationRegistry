using System.Threading.Tasks;
using IdentityServer4.Stores;
using iSHARE.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace iSHARE.IdentityServer.Services
{
    internal class SigningCredentialsStore : ISigningCredentialStore
    {
        private readonly IPrivateKeyVault _privateKeyVault;

        public SigningCredentialsStore(IPrivateKeyVault privateKeyVault)
        {
            _privateKeyVault = privateKeyVault;
        }

        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var privateKey = await _privateKeyVault.GetRsaPrivateKey();
            var securityKey = new RsaSecurityKey(privateKey.ConvertToRsa());

            return new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        }
    }
}
