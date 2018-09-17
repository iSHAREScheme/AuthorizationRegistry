using System;

namespace NLIP.iShare.AuthorizationRegistry.Core.Requests
{
    public class SendEmailActivationUserRequest : UserModelRequest
    {
        public Guid Id { get; set; }
    }
}
