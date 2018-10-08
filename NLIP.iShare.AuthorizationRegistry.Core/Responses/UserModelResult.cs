using NLIP.iShare.Models.Responses;

namespace NLIP.iShare.AuthorizationRegistry.Core.Responses
{
    public class UserModelResult : RequestResultModel<UserModel>
    {
        public UserModelResult() { }
        public UserModelResult(string error) : base(error) { }
    }
}
