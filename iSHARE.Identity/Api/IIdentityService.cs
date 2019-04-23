using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using iSHARE.Identity.Requests;
using iSHARE.Models;

namespace iSHARE.Identity
{
    public interface IIdentityService
    {
        Task<Response<UserModel>> ConfirmPasswordReset(ConfirmPasswordResetRequest request);
        Task<Response> ForcePasswordReset(Guid userId, ClaimsPrincipal principal);
        Task<Response<UserModel>> ChangePassword(ChangePasswordRequest request, string aspNetUserId);
        Task<bool> Exists(Guid id);
        Task<Response> MakeInactive(Guid id);
        Task<Response<UserModel>> ActivateAccountConfirm(ActivateAccountRequest request);
        Task<Response<UserModel>> ActivateAccountSendEmail(SendEmailActivationUserRequest request, ClaimsPrincipal principal);
        Task<Response<UserModel>> SendForgotPasswordEmail(ForgotPasswordUserRequest request);
        Task<UserModel> Get(Guid id, ClaimsPrincipal principal);
        Task<IReadOnlyCollection<AspNetUser>> GetIdentities(IReadOnlyCollection<string> identitiesIds);
        Task<Response<UserModel>> Create(CreateUserRequest request, ClaimsPrincipal principal);
        Task<Response<UserModel>> Update(UpdateUserRequest request, ClaimsPrincipal principal);
    }
}
