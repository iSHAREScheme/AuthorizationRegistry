using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using iSHARE.Configuration;
using iSHARE.Models;
using Microsoft.Extensions.Logging;

namespace iSHARE.IdentityServer.Stores
{
    /// <summary>
    /// Enables Identity Server the capability to support OAuth clients "on the fly"
    /// </summary>
    internal class PublicClientStore : IClientStore
    {
        private readonly Decorator<IClientStore> _clientStore;
        private readonly ILogger _logger;

        public PublicClientStore(Decorator<IClientStore> clientStore, ILogger<PublicClientStore> logger)
        {
            _clientStore = clientStore;
            _logger = logger;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _clientStore.Instance.FindClientByIdAsync(clientId);

            if (client != null)
            {
                return client;
            }

            _logger.LogInformation("Client {clientId} is not found in the store, return a clone of the requested client.", clientId);

            return new Client
            {
                ClientId = clientId,
                AllowedGrantTypes = new List<string> { OidcConstants.GrantTypes.ClientCredentials },
                AllowedScopes = new List<string> { StandardScopes.iSHARE }
            };
        }
    }
}
