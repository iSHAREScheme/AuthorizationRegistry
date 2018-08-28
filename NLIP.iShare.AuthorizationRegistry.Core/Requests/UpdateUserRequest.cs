using System;

namespace NLIP.iShare.AuthorizationRegistry.Core.Requests
{
    public class UpdateUserRequest : UserModelRequest
    {
        public Guid Id { get; set; }
    }
}
