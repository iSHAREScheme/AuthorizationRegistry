using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using NLIP.iShare.Models;

namespace NLIP.iShare.Identity
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
            var identity = await _userManager.GetUserAsync(context.Subject).ConfigureAwait(false);

            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(identity.Email))
            {
                claims.Add(new Claim("email", identity.Email));
            }

            var extraClaimsData = await _extraClaimsProvider(identity.Id).ConfigureAwait(false);

            foreach (var claim in extraClaimsData.Where(c => !string.IsNullOrEmpty(c.Item2)))
            {
                if (!string.IsNullOrEmpty(claim.Item2))
                {
                    claims.Add(new Claim(claim.Item1, claim.Item2));
                }
            }

            var roles = await _userManager.GetRolesAsync(identity).ConfigureAwait(false);

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
