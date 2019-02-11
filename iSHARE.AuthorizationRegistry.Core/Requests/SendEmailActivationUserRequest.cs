using System;

namespace iSHARE.AuthorizationRegistry.Core.Requests
{
    public class SendEmailActivationUserRequest : UserModelRequest
    {
        public Guid Id { get; set; }
    }
}
