using iSHARE.IdentityServer.UserInfo;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.IdentityServer
{
    /// <summary>
    /// Please read !README.txt under UserInfo directory
    /// </summary>
    public static class UserInfoConfiguration
    {
        public static IIdentityServerBuilder AddMockedUserInfoEndpoint(this IIdentityServerBuilder builder)
        {
            AddDependencies(builder.Services);

            builder.AddEndpoint<UserInfoEndpoint>("UserinfoMock", "/connect/userinfo_mock");

            return builder;
        }

        private static void AddDependencies(IServiceCollection services)
        {
            services.AddTransient<IBearerTokenUsageValidator, BearerTokenUsageValidator>();
            services.AddTransient<ICustomUserInfoEndpointValidator, CustomUserInfoEndpointValidator>();
            services.AddTransient<IUserInfoResponseGenerator, ResponseGeneratorMock>();
        }
    }
}
