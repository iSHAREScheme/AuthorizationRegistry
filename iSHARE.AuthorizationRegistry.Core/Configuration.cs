using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.Configuration.Configurations;
using iSHARE.Identity;
using iSHARE.Identity.Api;
using iSHARE.Identity.Login;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.AuthorizationRegistry.Core
{
    public static class Configuration
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDelegationService, DelegationService>();
            services.AddTransient<IDelegationValidationService, DelegationValidationService>();
            services.AddTransient<ITenantUserBuilder, TenantUserBuilder>();
            var identityProviderOptions = services.BuildServiceProvider().GetRequiredService<SchemeOwnerIdentityProviderOptions>();
            if (identityProviderOptions.Enable)
            {
                services.AddTransient<IUserHandler, UserHandler>();
                services.AddTransient<IIdentityService, NullIdentityService>();
                services.AddTransient(typeof(IAccountService<AspNetUser>), typeof(NullAccountService<AspNetUser>));
            }
            else
            {
                services.AddTransient<IUsersService, UsersService>();
                services.AddTransient<IIdentityService, IdentityService>();

                services.AddTransient(opts =>
                {
                    Func<string, Task<IReadOnlyCollection<ValueTuple<string, string>>>> func = identityUserId =>
                    {
                        var usersService = opts.GetService<IUsersService>();

                        return usersService.GetClaims(identityUserId);
                    };
                    return func;
                });
            }

            return services;
        }
    }
}
