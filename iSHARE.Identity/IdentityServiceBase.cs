using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using iSHARE.Abstractions.Email;
using iSHARE.Configuration.Configurations;
using iSHARE.Identity.Requests;
using iSHARE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iSHARE.Identity
{
    public class IdentityServiceBase
    {
        protected readonly ITenantUsersRepository _usersRepository;
        protected readonly UserManager<AspNetUser> _userManager;
        protected readonly EmailTemplatesData _templateData;
        protected readonly ITemplateService _templateService;
        protected readonly IEmailClient _emailClient;
        protected readonly ILogger _logger;
        protected readonly SpaOptions _spaOptions;
        public IdentityServiceBase(ITenantUsersRepository usersRepository,
            UserManager<AspNetUser> userManager,
            ILogger logger,
            EmailTemplatesData templateData,
            ITemplateService templateService,
            IEmailClient emailClient,
            SpaOptions spaOptions)
        {
            _usersRepository = usersRepository;
            _userManager = userManager;
            _logger = logger;
            _templateData = templateData;
            _templateService = templateService;
            _emailClient = emailClient;
            _spaOptions = spaOptions;
        }

        public async Task<UserModel> Get(Guid id, ClaimsPrincipal principal)
        {
            var user = await _usersRepository.GetById(id);
            if (user != null && (user.PartyId == principal.GetPartyId() || principal.IsSchemeOwner()))
            {
                var identity = await _userManager.Users.FirstAsync(u => u.Id == user.AspNetUserId);
                var roles = await _userManager.GetRolesAsync(identity);

                return UserModel.Create(identity, user, roles.ToArray());
            }
            return null;
        }
        public async Task<IReadOnlyCollection<AspNetUser>> GetIdentities(IReadOnlyCollection<string> identitiesIds)
        {
            var identities = await _userManager.Users
                    .Where(identity => identitiesIds.Contains(identity.Id))
                    .ToListAsync()
                ;

            foreach (var identity in identities)
            {
                var roles = await _userManager.GetRolesAsync(identity);
                identity.Roles = roles.ToList();
            }

            return identities;
        }

        public async Task<Response<UserModel>> SendForgotPasswordEmail(ForgotPasswordUserRequest request)
        {
            var identity = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (identity == null)
            {
                _logger.LogWarning($"Username {request.Email} does not exist.");
                return Response<UserModel>.ForSuccess();
            }

            var user = await _usersRepository.GetByIdentity(identity.Id);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identity);

            var template = await GetUserEmailAsync(identity, user, _templateData.ForgotPasswordEmailFileName, "reset-password", token);

            await _emailClient.Send(identity.Email, "Forgot password notification", template);

            _logger.LogInformation($"Reset password email sent for {request.Email}.");

            return Response<UserModel>.ForSuccess();
        }
        public async Task<Response<UserModel>> ActivateAccountSendEmail(SendEmailActivationUserRequest request, ClaimsPrincipal principal)
        {
            var user = await _usersRepository.GetById(request.Id);

            if (user != null && user.Active)
            {
                return Response<UserModel>.ForError("Account already activated.");
            }

            if (user.PartyId != principal.GetPartyId() && !principal.IsSchemeOwner())
            {
                return Response<UserModel>.ForError("Missing permisssions.");
            }

            var identity = await _userManager.Users.FirstAsync(u => u.Id == user.AspNetUserId);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identity);

            var template = await GetUserEmailAsync(identity, user, _templateData.ActivateAccountEmailFileName, "activate", token);

            await _emailClient.Send(identity.Email, "Activate account notification", template);

            return Response<UserModel>.ForSuccess();
        }

        public async Task<Response<UserModel>> ActivateAccountConfirm(ActivateAccountRequest request)
        {
            if (!request.Id.HasValue)
            {
                return Response<UserModel>.ForError("Request id is required.");
            }

            var user = await _usersRepository.GetById(request.Id.Value);
            if (user == null)
            {
                return Response<UserModel>.ForError("Associated user not found.");
            }

            if (user.Active)
            {
                return Response<UserModel>.ForError("Account already activated.");
            }

            var aspNetUser = await _userManager.FindByIdAsync(user.AspNetUserId);

            var tokenResult = await _userManager.ConfirmEmailAsync(aspNetUser, request.Token);
            if (!tokenResult.Succeeded)
            {
                return ConvertToResponse(tokenResult);
            }

            var passwordResult = await _userManager.AddPasswordAsync(aspNetUser, request.Password);

            if (!passwordResult.Succeeded)
            {
                _logger.LogWarning("account activation failed: {0}", passwordResult.Errors);
            }
            else
            {
                user.Active = true;
                await _usersRepository.Save();
            }

            return ConvertToResponse(passwordResult);
        }

        public async Task<Response> MakeInactive(Guid id)
        {
            var user = await _usersRepository.GetById(id);

            if (user == null)
            {
                return Response.ForError("User not found.");
            }

            var identity = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == user.AspNetUserId);

            user.Deleted = true;
            identity.IsDeleted = true;

            await _usersRepository.Save();
            await _userManager.UpdateAsync(identity);

            return Response.ForSuccess();
        }

        public async Task<bool> Exists(Guid id)
        {
            var user = await _usersRepository.GetById(id);
            var identityExists = await _userManager.Users.AnyAsync(u => u.Id == user.AspNetUserId);

            return user != null && identityExists;
        }

        public async Task<Response<UserModel>> ChangePassword(ChangePasswordRequest request, string aspNetUserId)
        {
            var aspNetUser = await _userManager.FindByIdAsync(aspNetUserId);
            if (aspNetUser == null)
            {
                return Response<UserModel>.ForError("User not found.");
            }

            if (request.NewPassword == request.CurrentPassword)
            {
                return Response<UserModel>.ForError("The new password should be different from the current password.");
            }

            var passwordValidationResult = await GetPasswordErrors(aspNetUser, request.NewPassword);
            if (!passwordValidationResult.Success)
            {
                return passwordValidationResult;
            }

            var result = await _userManager.ChangePasswordAsync(aspNetUser, request.CurrentPassword, request.NewPassword);
            return ConvertToResponse(result);
        }

        public async Task<Response<UserModel>> ConfirmPasswordReset(ConfirmPasswordResetRequest request)
        {
            if (!request.Id.HasValue)
            {
                return Response<UserModel>.ForError("Request id is required.");
            }

            var user = await _usersRepository.GetById(request.Id.Value);
            if (user == null)
            {
                return Response<UserModel>.ForError("User not found.");
            }

            var identity = await _userManager.FindByIdAsync(user.AspNetUserId);

            var validationResult = await GetPasswordErrors(identity, request.NewPassword);
            if (!validationResult.Success)
            {
                return validationResult;
            }
            var result = await _userManager.ConfirmEmailAsync(identity, request.Token);
            if (!result.Succeeded)
            {
                return ConvertToResponse(result);
            }

            result = await _userManager.RemovePasswordAsync(identity);
            if (!result.Succeeded)
            {
                return ConvertToResponse(result);
            }

            result = await _userManager.AddPasswordAsync(identity, request.NewPassword);
            if (!result.Succeeded)
            {
                return ConvertToResponse(result);
            }

            user.Active = true;
            await _usersRepository.Save();

            return Response<UserModel>.ForSuccess();
        }

        public async Task<Response> ForcePasswordReset(Guid userId, ClaimsPrincipal principal)
        {
            var user = await _usersRepository.GetById(userId);
            if (user == null)
            {
                return Response.ForError("User not found.");
            }
            if (user.PartyId != principal.GetPartyId() && !principal.IsSchemeOwner())
            {
                return Response.ForError("Missing permisssions.");
            }
            user.Active = false;
            await _usersRepository.Save();

            var identity = await _userManager.FindByIdAsync(user.AspNetUserId);
            if (await _userManager.HasPasswordAsync(identity))
            {
                await _userManager.RemovePasswordAsync(identity);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identity);
            var email = await GetUserEmailAsync(identity, user, _templateData.ResetPasswordEmailFileName, "reset-password", token);

            await _emailClient.Send(identity.Email, "Password change required by administrator", email);

            return Response.ForSuccess();
        }

        private static Response<UserModel> ConvertToResponse(IdentityResult result)
            => result.Succeeded ? Response<UserModel>.ForSuccess()
                : Response<UserModel>.ForErrors(result.Errors.Select(e => e.Description));

        private async Task<Response<UserModel>> GetPasswordErrors(AspNetUser identity, string password)
        {
            var results = await Task
                .WhenAll(_userManager.PasswordValidators.Select(v =>
                    v.ValidateAsync(_userManager, identity, password)));

            var errorMessages = results.SelectMany(r => r.Errors).Select(i => i.Description).ToArray();
            if (errorMessages.Any())
            {
                return Response<UserModel>.ForErrors(errorMessages);
            }
            return Response<UserModel>.ForSuccess();
        }

        private async Task<string> GetUserEmailAsync(AspNetUser identity, IUser user, string fileName, string action, string token)
        {
            _templateData.EmailData.Add("Username", identity.UserName);

            var encodedToken = HttpUtility.UrlEncode(token);
            var actionUrl = $"{_spaOptions.BaseUri}account/{action}?uid={user.Id}&token={encodedToken}";

            _templateData.EmailData.Add("ActionUrl", actionUrl);

            return await _templateService.GetTransformed(fileName, _templateData.EmailData);
        }
    }
}
