using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.Identity;
using iSHARE.Identity.Requests;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core
{
    public class NullIdentityService : IIdentityService
    {
        public Task<Response<UserModel>> ConfirmPasswordReset(ConfirmPasswordResetRequest request)
        {
            Response<UserModel> response = Response.ForError("");
            return Task.FromResult(response);
        }

        public Task<Response> ForcePasswordReset(Guid userId, ClaimsPrincipal principal)
        {
            return Task.FromResult(Response.ForError(""));
        }

        public Task<Response<UserModel>> ChangePassword(ChangePasswordRequest request, string aspNetUserId)
        {
            Response<UserModel> response = Response.ForError("");
            return Task.FromResult(response);
        }

        public Task<bool> Exists(Guid id)
        {
            return Task.FromResult(false);
        }

        public Task<Response> MakeInactive(Guid id, ClaimsPrincipal principal)
        {
            return Task.FromResult(Response.ForError(""));
        }

        public Task<Response<UserModel>> ActivateAccountConfirm(ActivateAccountRequest request)
        {
            Response<UserModel> response = Response.ForError("");
            return Task.FromResult(response);
        }

        public Task<Response<UserModel>> ActivateAccountSendEmail(SendEmailActivationUserRequest request, ClaimsPrincipal principal)
        {
            Response<UserModel> response = Response.ForError("");
            return Task.FromResult(response);
        }

        public Task<Response<UserModel>> SendForgotPasswordEmail(ForgotPasswordUserRequest request)
        {
            Response<UserModel> response = Response.ForError("");
            return Task.FromResult(response);
        }

        public Task<UserModel> Get(Guid id, ClaimsPrincipal principal)
        {
            UserModel user = null;
            return Task.FromResult(user);
        }

        public Task<IReadOnlyCollection<AspNetUser>> GetIdentities(IReadOnlyCollection<string> identitiesIds)
        {
            IReadOnlyCollection<AspNetUser> list = new List<AspNetUser>();
            return Task.FromResult(list);
        }

        public Task<Response<UserModel>> Create(CreateUserRequest request, ClaimsPrincipal principal)
        {
            Response<UserModel> response = Response.ForError("");
            return Task.FromResult(response);
        }

        public Task<Response<UserModel>> Update(UpdateUserRequest request, ClaimsPrincipal principal)
        {
            Response<UserModel> response = Response.ForError("");
            return Task.FromResult(response);
        }
    }
}
