using NLIP.iShare.Abstractions;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.AuthorizationRegistry.Core.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLIP.iShare.Models;

namespace NLIP.iShare.AuthorizationRegistry.Core.Api
{
    /// <summary>
    /// Defines the use cases related to users retrieval and their management
    /// </summary>
    public interface IUsersService
    {        
        Task<UserModel> Get(Guid id);        
        Task<IReadOnlyCollection<(string, string)>> GetClaims(string identityUserId);
        Task<PagedResult<UserModel>> GetAll(Query query);
        Task<Response> MakeInactive(Guid id);
        Task<Response<UserModel>> Create(CreateUserRequest request);
        Task<Response<UserModel>> Update(UpdateUserRequest request);
        Task<bool> Exists(Guid id);
        Task<Response<UserModel>> ActivateAccountSendEmail(SendEmailActivationUserRequest request);
        Task<Response<UserModel>> ChangePassword(ChangePasswordRequest request, string aspNetUserId);
        Task<Response<UserModel>> ActivateAccountConfirm(ActivateAccountRequest request);
        Task<Response<UserModel>> SendForgotPasswordEmail(ForgotPasswordUserRequest request);
        Task<Response> ForcePasswordReset(Guid userId);
        Task<Response<UserModel>> ConfirmPasswordReset(ConfirmPasswordResetRequest request);
    }
}
