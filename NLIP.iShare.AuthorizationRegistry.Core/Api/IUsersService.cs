using NLIP.iShare.Api.Responses;
using NLIP.iShare.AuthorizationRegistry.Core.Requests;
using NLIP.iShare.AuthorizationRegistry.Core.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLIP.iShare.Abstractions;

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
        Task<RequestResult> MakeInactive(Guid id);
        Task<UserModelResult> Create(CreateUserRequest request);
        Task<UserModelResult> Update(UpdateUserRequest request);
        Task<bool> Exists(Guid id);
        Task<UserModelResult> ActivateAccountSendEmail(SendEmailActivationUserRequest request);
        Task<UserModelResult> ChangePassword(ChangePasswordRequest request, string aspNetUserId);
        Task<UserModelResult> ActivateAccountConfirm(ActivateAccountRequest request);
        Task<UserModelResult> SendForgotPasswordEmail(ForgotPasswordUserRequest request);
        Task<UserModelResult> ForcePasswordReset(Guid userId);
        Task<UserModelResult> ConfirmPasswordReset(ConfirmPasswordResetRequest request);
    }
}
