using System;

namespace iSHARE.AuthorizationRegistry.Core.Requests
{
    public class UpdateUserRequest : UserModelRequest
    {
        public Guid Id { get; set; }
    }
}
