using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Data.Models;
using iSHARE.Identity.Login;

namespace iSHARE.AuthorizationRegistry.Core
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
