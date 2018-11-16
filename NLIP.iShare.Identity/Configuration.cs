using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLIP.iShare.Identity.Login;
using NLIP.iShare.Models;

namespace NLIP.iShare.Identity
{
    public static class Configuration
    {
        public static IIdentityServerBuilder AddIdentity<TUser, TUserStore>(this IIdentityServerBuilder identityServerBuilder)
            where TUser: class, IAspNetUser
            where TUserStore: DbContext
        {
            var services = identityServerBuilder.Services;

            services.AddTransient(typeof(IAccountService<TUser>), typeof(AccountService<TUser>));
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(1);
            });

            services.AddIdentity<TUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<TUserStore>()
                ;

            
            identityServerBuilder
                .AddAspNetIdentity<TUser>()
                .AddProfileService<ProfileService<TUser>>()
                ;
            return identityServerBuilder;
        }
    }
}
