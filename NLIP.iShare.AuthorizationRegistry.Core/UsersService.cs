using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using NLIP.iShare.Abstractions;
using NLIP.iShare.Abstractions.Email;
using NLIP.iShare.Api.Responses;
using NLIP.iShare.AuthorizationRegistry.Core.Api;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.AuthorizationRegistry.Core.Responses;
using NLIP.iShare.AuthorizationRegistry.Data;
using NLIP.iShare.AuthorizationRegistry.Data.Models;
using NLIP.iShare.Configuration.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NLIP.iShare.AuthorizationRegistry.IdentityServer.Models;
using NLIP.iShare.Models;


namespace NLIP.iShare.AuthorizationRegistry.Core
{
    public class UsersService : IUsersService
    {
        private readonly AuthorizationRegistryDbContext _db;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ITemplateService _templateService;
        private readonly IEmailClient _emailClient;
        private readonly ILogger<UsersService> _logger;
        private readonly SpaOptions _spaOptions;
        private readonly EmailTemplatesData _templateData;

        public UsersService(AuthorizationRegistryDbContext db,
            UserManager<AspNetUser> userManager,
            ITemplateService templateService,
            IEmailClient emailClient,
            EmailTemplatesData templateData,
            ILogger<UsersService> logger, 
            SpaOptions spaOptions)
        {
            _templateData = templateData;
            _emailClient = emailClient;
            _templateService = templateService;
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _spaOptions = spaOptions;
        }

        public async Task<PagedResult<UserModel>> GetAll(Query query)
        {
            var usersQuery = _db.Users.Where(u => !u.Deleted);
            int count = await usersQuery.CountAsync().ConfigureAwait(false);
            usersQuery = Sort(query, usersQuery).GetPage(query);

            var users = await usersQuery.ToListAsync().ConfigureAwait(false);
            var identities = await _userManager.Users
                                         .Where(identity => users.Any(user => user.AspNetUserId == identity.Id))
                                         .ToListAsync()
                                         .ConfigureAwait(false);

            foreach (var identity in identities)
            {
                var roles = await _userManager.GetRolesAsync(identity).ConfigureAwait(false);
                identity.Roles = roles.ToList();
            }

            var mappedUsers = from user in users
                              join identity in identities on user.AspNetUserId equals identity.Id
                              select new UserModel
                              {
                                  Id = user.Id,
                                  Username = identity.UserName,
                                  PartyId = user.PartyId,
                                  PartyName = user.PartyName,
                                  CreatedDate = user.CreatedDate,
                                  Roles = identity.Roles.ToArray(),
                                  IdentityId = identity.Id,
                                  Active = user.Active

                              };

            return mappedUsers.ToPagedResult(count);
        }

        public async Task<UserModel> Get(Guid id)
        {
            var user = await _db.Users.FirstAsync(u => u.Id == id).ConfigureAwait(false);
            var identity = await _userManager.Users.FirstAsync(u => u.Id == user.AspNetUserId).ConfigureAwait(false);
            var roles = await _userManager.GetRolesAsync(identity).ConfigureAwait(false);

            return UserModel.Create(identity, user, roles.ToArray());
        }


        private UserModelResult ValidateRequest(UserModelRequest request)
        {
            var isSchemeOwner = request.Roles.IndexOf(Constants.Roles.SchemeOwner) >= 0;
            if (!isSchemeOwner || (isSchemeOwner && request.Roles.Length > 1))
            {
                if (string.IsNullOrWhiteSpace(request.PartyId))
                {
                    return new UserModelResult { Success = false, Errors = new[] { "Party ID cannot be empty." } };
                }

                if (string.IsNullOrWhiteSpace(request.PartyName))
                {
                    return new UserModelResult { Success = false, Errors = new[] { "Party Name cannot be empty." } };
                }
            }
            else if (isSchemeOwner)
            {
                // party ID and party Name aren't used for SchemOwner role
                request.PartyId = string.Empty;
                request.PartyName = string.Empty;
            }
            return new UserModelResult { Success = true };
        }

        public async Task<UserModelResult> SendForgotPasswordEmail(ForgotPasswordUserRequest request)
        {
            var identity = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
            if (identity == null)
            {
                return new UserModelResult("Username does not exist!");
            }
            var user = await _db.Users.FirstOrDefaultAsync(u => u.AspNetUserId == identity.Id);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identity);

            var template = await GetUserEmailAsync(identity, user, _templateData.ForgotPasswordEmailFileName, "reset-password", token);

            await _emailClient.Send(identity.Email, "Forgot password notification", template);

            return new UserModelResult { Success = true };
        }

        private async Task<string> GetUserEmailAsync(AspNetUser identity, User user, string fileName, string action, string token)
        {
            _templateData.EmailData.Add("Username", identity.UserName);

            var encodedToken = HttpUtility.UrlEncode(token);
            var actionUrl = $"{_spaOptions.BaseUri}account/{action}?uid={user.Id}&token={encodedToken}";

            _templateData.EmailData.Add("ActionUrl", actionUrl);

            return await _templateService.GetTransformed(fileName, _templateData.EmailData);
        }

        public async Task<UserModelResult> ActivateAccountSendEmail(SendEmailActivationUserRequest request)
        {
            var user = await _db.Users.FirstAsync(u => u.Id == request.Id).ConfigureAwait(false);

            if (user != null && user.Active)
            {
                return new UserModelResult("Account already activated!");
            }

            var identity = await _userManager.Users.FirstAsync(u => u.Id == user.AspNetUserId).ConfigureAwait(false);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identity);

            var template = await GetUserEmailAsync(identity, user, _templateData.ActivateAccountEmailFileName, "activate", token);

            await _emailClient.Send(identity.Email, "Activate account notification", template);

            return new UserModelResult { Success = true };
        }

        public async Task<UserModelResult> ActivateAccountConfirm(ActivateAccountRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null)
            {
                return new UserModelResult("Invalid request");
            }

            if (user.Active)
            {
                return new UserModelResult("Account already activated");
            }

            var aspNetUser = await _userManager.FindByIdAsync(user.AspNetUserId);

            var tokenResult = await _userManager.ConfirmEmailAsync(aspNetUser, request.Token);
            if (!tokenResult.Succeeded)
            {
                return Convert(tokenResult);
            }
            ////TODO: check if explicit password validation is actually required
            //var passwordValidationResult = await GetPasswordErrros(aspNetUser, request.Password);
            //if (!passwordValidationResult.Success)
            //{
            //    return passwordValidationResult;
            //}


            var passwordResult = await _userManager.AddPasswordAsync(aspNetUser, request.Password);

            if (!passwordResult.Succeeded)
            {
                _logger.LogWarning("account activation failed: {0}", passwordResult.Errors);
            }
            else
            {
                user.Active = true;
                await _db.SaveChangesAsync();
            }

            return Convert(passwordResult);
        }


        public async Task<UserModelResult> Create(CreateUserRequest request)
        {
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

            var result = await _userManager.CreateAsync(identity).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return new UserModelResult { Success = false, Errors = result.Errors.Select(e => e.Description).ToArray() };
            }

            foreach (var role in request.Roles)
            {
                await _userManager.AddToRoleAsync(identity, role).ConfigureAwait(false);
            }

            var user = new User
            {
                AspNetUserId = identity.Id,
                Name = request.Username,
                CreatedDate = DateTime.Now,
                PartyId = request.PartyId,
                PartyName = request.PartyName
            };

            await _db.Users.AddAsync(user).ConfigureAwait(false);
            await _db.SaveChangesAsync().ConfigureAwait(false);

            var emailRequest = new SendEmailActivationUserRequest { Id = user.Id };
            var sendEmail = await ActivateAccountSendEmail(emailRequest);

            if (sendEmail.Success)
            {
                return new UserModelResult
                {
                    Success = true,
                    Model = UserModel.Create(identity, user, request.Roles)
                };
            }

            try
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync().ConfigureAwait(false);
                await _userManager.DeleteAsync(identity).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError("unable to delete invalid account: ", ex);
                return new UserModelResult("unable to create account");
            }

            return new UserModelResult { Success = false, Errors = result.Errors.Select(e => e.Description).ToArray() };
        }

        public async Task<UserModelResult> Update(UpdateUserRequest request)
        {
            var user = await _db.Users.FirstAsync(u => u.Id == request.Id).ConfigureAwait(false);
            var identity = await _userManager.Users.FirstAsync(u => u.Id == user.AspNetUserId).ConfigureAwait(false);

            var validationResult = ValidateRequest(request);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            var currentRoles = await _userManager.GetRolesAsync(identity).ConfigureAwait(false);
            await _userManager.RemoveFromRolesAsync(identity, currentRoles).ConfigureAwait(false);
            await _userManager.AddToRolesAsync(identity, request.Roles).ConfigureAwait(false);

            user.PartyId = request.PartyId;
            user.PartyName = request.PartyName;

            _db.Users.Update(user);
            await _db.SaveChangesAsync().ConfigureAwait(false);

            return new UserModelResult
            {
                Success = true,
                Model = UserModel.Create(identity, user, request.Roles)
            };
        }

        public async Task<RequestResult> MakeInactive(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(false);

            if (user == null)
            {
                return new RequestResult("user id not found");
            }

            var identity = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == user.AspNetUserId);

            user.Deleted = true;
            identity.IsDeleted = true;

            _db.Users.Update(user);
            await _userManager.UpdateAsync(identity);

            await _db.SaveChangesAsync().ConfigureAwait(false);

            return new RequestResult { Success = true };
        }

        public async Task<bool> Exists(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.Deleted).ConfigureAwait(false);
            var identityExists = await _userManager.Users.AnyAsync(u => u.Id == user.AspNetUserId).ConfigureAwait(false);

            return user != null && identityExists;
        }

        public async Task<IReadOnlyCollection<(string, string)>> GetClaims(string identityUserId)
        {
            var user = await _db.Users.FirstAsync(u => u.AspNetUserId == identityUserId).ConfigureAwait(false);

            return new[]
            {
                ("partyId", user?.PartyId),
                ("partyName", user?.PartyName),
            };
        }

        private static IQueryable<User> Sort(Query query, IQueryable<User> data)
        {
            var result = data;

            if (query.SortBy == "partyId")
            {
                result = result.OrderBy(c => c.PartyId, query.SortOrder);
            }
            return result;
        }

        public async Task<UserModelResult> ChangePassword(ChangePasswordRequest request, string aspNetUserId)
        {
            var aspNetUser = await _userManager.FindByIdAsync(aspNetUserId);
            if (aspNetUser == null)
            {
                return new UserModelResult("user not found");
            }

            if (request.NewPassword == request.CurrentPassword)
            {
                return new UserModelResult("the new password should be different from the current password");
            }

            var passwordValidationResult = (await GetPasswordErrors(aspNetUser, request.NewPassword));
            if (!passwordValidationResult.Success)
            {
                return passwordValidationResult;
            }

            var result = await _userManager.ChangePasswordAsync(aspNetUser, request.CurrentPassword, request.NewPassword);
            return Convert(result);
        }

        private UserModelResult Convert(IdentityResult result)
        {
            if (result.Succeeded)
            {
                return new UserModelResult();
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return new UserModelResult { Success = false, Errors = errors };
        }

        private async Task<UserModelResult> GetPasswordErrors(AspNetUser identity, string password)
        {
            var results = await Task
                .WhenAll(_userManager.PasswordValidators.Select(v =>
                    v.ValidateAsync(_userManager, identity, password)));
            
            var errorMessages = results.SelectMany(r => r.Errors).Select(i => i.Description).ToArray();
            if (errorMessages.Any())
            {
                return new UserModelResult { Success = false, Errors = errorMessages };
            }
            return new UserModelResult();
        }


        public async Task<UserModelResult> ForcePasswordReset(Guid userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return new UserModelResult("user not found");
            }

            user.Active = false;
            await _db.SaveChangesAsync();

            var identity = await _userManager.FindByIdAsync(user.AspNetUserId);
            if (await _userManager.HasPasswordAsync(identity))
            {
                await _userManager.RemovePasswordAsync(identity);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(identity);
            var email = await GetUserEmailAsync(identity, user, _templateData.ResetPasswordEmailFileName, "reset-password", token);

            await _emailClient.Send(identity.Email, "Password change required by administrator", email);

            return new UserModelResult();
        }

        /// <summary>
        /// Used by <see cref="ForcePasswordReset"/> and <see cref="SendForgotPasswordEmail"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserModelResult> ConfirmPasswordReset(ConfirmPasswordResetRequest request)
        {
            var user = await _db.Users.FindAsync(request.Id);
            if (user == null)
            {
                return new UserModelResult("user not found");
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
                return Convert(result);
            }

            result = await _userManager.RemovePasswordAsync(identity);
            if (!result.Succeeded)
            {
                return Convert(result);
            }

            result = await _userManager.AddPasswordAsync(identity, request.NewPassword);
            if (!result.Succeeded)
            {
                return Convert(result);
            }

            user.Active = true;
            await _db.SaveChangesAsync();

            return new UserModelResult();
        }
    }
}
