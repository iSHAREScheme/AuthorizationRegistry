using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLIP.iShare.Abstractions.Email;

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
            services.AddSingleton(srv => new EmailTemplatesData
            {
                EmailData = new Dictionary<string, string>
                {
                    { "LogoUrl", configuration["OAuth2:AuthServerUrl"] + "images/ishare_logo.png"}
                }
            });
            services.AddScoped<IDelegationJwtBuilder, DelegationJwtBuilder>();

            services.AddScoped(opts =>
            {
                Func<string, Task<IReadOnlyCollection<ValueTuple<string, string>>>> func = identityUserId =>
                {
                    var usersService = opts.GetService<IUsersService>();

                    return usersService.GetClaims(identityUserId);
                };
                return func;
            });
            services.AddSingleton<ITemplateService, TemplateService>();
        }
    }
}
