using System;

namespace iSHARE.Identity.Requests
{
    public class UpdateUserRequest : UserModelRequest
    {
        public Guid Id { get; set; }
    }
}
