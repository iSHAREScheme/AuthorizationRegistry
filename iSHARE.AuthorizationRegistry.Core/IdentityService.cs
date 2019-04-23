using iSHARE.Abstractions;
using iSHARE.Abstractions.Email;
using iSHARE.Configuration.Configurations;
using iSHARE.Identity;
using iSHARE.Identity.Requests;
using iSHARE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Constants = iSHARE.Identity.Constants;

namespace iSHARE.AuthorizationRegistry.Core
{
    public class IdentityService : IdentityServiceBase, IIdentityService
    {
        private readonly ITenantUserBuilder _tenantUserBuilder;

        public IdentityService(UserManager<AspNetUser> userManager,
            ITenantUsersRepository usersRepository,
            ITenantUserBuilder tenantUserBuilder,
            IEmailClient emailClient,
            EmailTemplatesData templateData,
            ITemplateService templateService,
            SpaOptions spaOptions,
            ILogger<IIdentityService> logger)
            : base(usersRepository, userManager, logger, templateData, templateService, emailClient, spaOptions)
        {
            _tenantUserBuilder = tenantUserBuilder;
        }

        private static bool ValidateRoles(string[] roles, ClaimsPrincipal principal)
        {
            if (roles == null || !roles.Any())
            {
                return false;
            }
            var userRoles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            if (userRoles.Contains(Constants.Roles.SchemeOwner))
            {
                return true;
            }

            var allowedRoles = new List<string>();
            if (userRoles.Contains(iSHARE.Models.Constants.Roles.AuthorizationRegistry.PartyAdmin))
            {
                allowedRoles.Add(iSHARE.Models.Constants.Roles.AuthorizationRegistry.PartyAdmin);
                allowedRoles.Add(iSHARE.Models.Constants.Roles.AuthorizationRegistry.EntitledPartyCreator);
                allowedRoles.Add(iSHARE.Models.Constants.Roles.AuthorizationRegistry.EntitledPartyViewer);
            }

            return roles.All(x => allowedRoles.Any(y => y == x));
        }

        public async Task<Response<UserModel>> Create(CreateUserRequest request, ClaimsPrincipal principal)
        {
            var roleValidation = ValidateRoles(request.Roles, principal);
            if (!roleValidation)
            {
                return Response<UserModel>.ForError("Missing permissions.");
            }
            if (!principal.IsSchemeOwner())
            {
                request.PartyId = principal.GetPartyId();
                request.PartyName = principal.GetPartyName();
            }

            var identity = new AspNetUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Username,
                Email = request.Email,
            };

            var validateResult = ValidateRequest(request);
            if (!validateResult.Success)
            {
                return validateResult;
            }

            var result = await _userManager.CreateAsync(identity);

            if (!result.Succeeded)
            {
                return Response<UserModel>.ForErrors(result.Errors.Select(e => e.Description));
            }

            foreach (var role in request.Roles)
            {
                await _userManager.AddToRoleAsync(identity, role);
            }

            var user = (await _tenantUserBuilder.BuildUser(request, identity.Id)) as Core.Models.User;

            _usersRepository.Add(user);

            await _usersRepository.Save();

            var sendEmail = await ActivateAccountSendEmail(new SendEmailActivationUserRequest { Id = user.Id }, principal);

            if (sendEmail.Success)
            {
                var createdUser = await _usersRepository.GetById(user.Id);
                return Response<UserModel>.ForSuccess(UserModel.Create(identity, createdUser, request.Roles));
            }

            try
            {
                await _usersRepository.Remove(user);
                await _usersRepository.Save();
                await _userManager.DeleteAsync(identity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to delete invalid account: ", ex);
                return Response<UserModel>.ForError("Unable to create account.");
            }

            return Response<UserModel>.ForErrors(result.Errors.Select(e => e.Description));
        }

        public async Task<Response<UserModel>> Update(UpdateUserRequest request, ClaimsPrincipal principal)
        {
            var roleValidation = ValidateRoles(request.Roles, principal);
            if (!roleValidation)
            {
                return Response<UserModel>.ForError("Missing permissions.");
            }
            if (!principal.IsSchemeOwner())
            {
                request.PartyId = principal.GetPartyId();
                request.PartyName = principal.GetPartyName();
            }

            var user = await _usersRepository.GetById(request.Id);
            var identity = await _userManager.Users.FirstAsync(u => u.Id == user.AspNetUserId);

            var validationResult = ValidateRequest(request);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            var currentRoles = await _userManager.GetRolesAsync(identity);
            await _userManager.RemoveFromRolesAsync(identity, currentRoles);
            await _userManager.AddToRolesAsync(identity, request.Roles);

            user.PartyId = request.PartyId;
            user.PartyName = request.PartyName;

            await _usersRepository.Save();

            return Response<UserModel>.ForSuccess(UserModel.Create(identity, user, request.Roles));
        }

        protected static Response<UserModel> ValidateRequest(UserModelRequest request)
        {
            var isSchemeOwner = request.Roles.Any() && request.Roles.Has(Constants.Roles.SchemeOwner) && request.Roles.Length == 1;
            if (isSchemeOwner)
            {
                request.PartyId = iSHARE.Models.Constants.SchemeOwnerPartyId;
                request.PartyName = iSHARE.Models.Constants.SchemeOwnerPartyName;
                return Response<UserModel>.ForSuccess();
            }

            if (string.IsNullOrWhiteSpace(request.PartyId))
            {
                return Response<UserModel>.ForError("Party ID cannot be empty.");
            }

            return Response<UserModel>.ForSuccess();
        }
    }
}
