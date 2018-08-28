using System;
using System.Collections.Generic;
using System.Text;

namespace NLIP.iShare.AuthorizationRegistry.Core.Requests
{
    public class SendEmailActivationUserRequest : UserModelRequest
    {
        public Guid Id { get; set; }
    }
}
