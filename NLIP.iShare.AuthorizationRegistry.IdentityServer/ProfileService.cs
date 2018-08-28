using System;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NLIP.iShare.AuthorizationRegistry.IdentityServer
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly Func<string, Task<IReadOnlyCollection<(string, string)>>> _extraClaimsProvider;

        public ProfileService(UserManager<AspNetUser> userManager, Func<string, Task<IReadOnlyCollection<(string, string)>>> extraClaimsProvider)
        {
            _userManager = userManager;
            _extraClaimsProvider = extraClaimsProvider;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var identity = await _userManager.GetUserAsync(context.Subject).ConfigureAwait(false);

            var claims = new List<Claim>
            {
                new Claim("email", identity.Email)
            };

            var extraClaimsData = await _extraClaimsProvider(identity.Id).ConfigureAwait(false);

            foreach (var claim in extraClaimsData.Where(c => !string.IsNullOrEmpty(c.Item2)))
            {
                claims.Add(new Claim(claim.Item1, claim.Item2));
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
