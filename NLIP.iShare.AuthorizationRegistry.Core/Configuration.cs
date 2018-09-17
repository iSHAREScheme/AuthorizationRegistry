using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Abstractions;
using NLIP.iShare.Abstractions.Email;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.Configuration.Configurations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NLIP.iShare.AuthorizationRegistry.Core
{
    public static class Configuration
    {
        public static void AddAuthorizationRegistryCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDelegationService, DelegationService>();
            services.AddScoped<IDelegationValidationService, DelegationValidationService>();
            services.AddScoped<IDelegationMaskValidationService, DelegationMaskValidationService>();
            services.AddScoped<IDelegationTranslateService, DelegationTranslateService>();         
            services.AddScoped<IUsersService, UsersService>();           

            services.AddScoped(opts =>
            {
                Func<string, Task<IReadOnlyCollection<ValueTuple<string, string>>>> func = identityUserId =>
                {
                    var usersService = opts.GetService<IUsersService>();

                    return usersService.GetClaims(identityUserId);
                };
                return func;
            });            
        }
    }
}
