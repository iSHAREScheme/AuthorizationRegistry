using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.Identity.Login;

namespace NLIP.iShare.AuthorizationRegistry.Core
{
    public static class Configuration
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {   
            services.AddTransient<IDelegationService, DelegationService>();
            services.AddTransient<IDelegationValidationService, DelegationValidationService>();            
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient(typeof(IAccountService<AspNetUser>), typeof(AccountService<AspNetUser>));

            services.AddTransient(opts =>
            {
                Func<string, Task<IReadOnlyCollection<ValueTuple<string, string>>>> func = identityUserId =>
                {
                    var usersService = opts.GetService<IUsersService>();

                    return usersService.GetClaims(identityUserId);
                };
                return func;
            });
            return services;
        }
    }
}
