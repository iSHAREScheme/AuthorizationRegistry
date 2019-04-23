using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using iSHARE.Models;
using Microsoft.AspNetCore.Identity;

namespace iSHARE.Identity
{
    public class ProfileService<TUser> : IProfileService
        where TUser : class, IAspNetUser
    {
        private readonly UserManager<TUser> _userManager;
        private readonly Func<string, Task<IReadOnlyCollection<(string, string)>>> _extraClaimsProvider;

        public ProfileService(UserManager<TUser> userManager, Func<string, Task<IReadOnlyCollection<(string, string)>>> extraClaimsProvider)
        {
            _userManager = userManager;
            _extraClaimsProvider = extraClaimsProvider;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var identity = await _userManager.GetUserAsync(context.Subject);

            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(identity.Email))
            {
                claims.Add(new Claim("email", identity.Email));
            }

            var extraClaimsData = await _extraClaimsProvider(identity.Id);

            foreach (var claim in extraClaimsData.Where(c => !string.IsNullOrEmpty(c.Item2)))
            {
                if (!string.IsNullOrEmpty(claim.Item2))
                {
                    claims.Add(new Claim(claim.Item1, claim.Item2));
                }
            }

            var roles = await _userManager.GetRolesAsync(identity);

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            context.IssuedClaims.AddRange(claims);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            return Task.CompletedTask;
        }
    }
}
