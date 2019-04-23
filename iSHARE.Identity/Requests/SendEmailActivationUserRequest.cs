using System;

namespace iSHARE.Identity.Requests
{
    public class SendEmailActivationUserRequest : UserModelRequest
    {
        public Guid Id { get; set; }
    }
}
