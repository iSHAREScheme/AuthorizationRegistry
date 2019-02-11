namespace iSHARE.AuthorizationRegistry.Core.Requests
{
    public class UserModelRequest
    {
        public string Email { get; set; }
        public string PartyId { get; set; }
        public string PartyName { get; set; }
        public string[] Roles { get; set; }
    }
}
