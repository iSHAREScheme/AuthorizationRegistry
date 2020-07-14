using System.Linq;
using System.Threading.Tasks;
using iSHARE.Abstractions;
using Microsoft.Azure.KeyVault;

namespace iSHARE.AzureKeyVaultClient
{
    internal class KeyVault : IDigitalSigner
    {
        private readonly KeyVaultClient _keyVaultClient;
        private readonly KeyVaultOptions _keyVaultOptions;

        public KeyVault(KeyVaultClient keyVaultClient, KeyVaultOptions keyVaultOptions)
        {
            _keyVaultClient = keyVaultClient;
            _keyVaultOptions = keyVaultOptions;
        }
        
        public async Task<byte[]> SignAsync(string algorithm, byte[] digest)
        {
            return (await _keyVaultClient.SignAsync(_keyVaultOptions.KeyIdentifier, algorithm, digest)).Result;
        }

        public async Task<bool> VerifyAsync(string algorithm, byte[] digest, byte[] signature)
        {
            return await _keyVaultClient.VerifyAsync(_keyVaultOptions.KeyIdentifier, algorithm, digest, signature);
        }

        public async Task<string> GetPublicKey()
        {
            var secretBundle = await _keyVaultClient.GetSecretAsync(_keyVaultOptions.KeyVaultUri, _keyVaultOptions.PublicKeySecretName);

            return secretBundle.Value;
        }

        public async Task<string[]> GetPublicKeyChain()
        {
            if (string.IsNullOrEmpty(_keyVaultOptions.PublicKeyChainSecretNames))
            {
                return new string[0];
            }

            var secretNames = _keyVaultOptions.PublicKeyChainSecretNames.Split(';');

            var tasks = secretNames
                .Select(secretName => _keyVaultClient.GetSecretAsync(_keyVaultOptions.KeyVaultUri, secretName))
                .ToArray();
            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result.Value).ToArray();
        }
    }
}
