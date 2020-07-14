using IdentityServer4.Stores;
using IdentityServer4.Stores.Default;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.IdentityServer
{
    public static class AuthorizationParametersStoreConfiguration
    {
        public static IIdentityServerBuilder AddInMemoryAuthorizationParametersMessageStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IAuthorizationParametersMessageStore, DistributedCacheAuthorizationParametersMessageStore>();

            return builder;
        }
    }
}
