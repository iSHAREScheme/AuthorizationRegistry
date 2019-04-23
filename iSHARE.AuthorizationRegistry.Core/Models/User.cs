using iSHARE.Identity;
using iSHARE.Models;

namespace iSHARE.AuthorizationRegistry.Core.Models
{
    public class User : Identity.User, IUser, IEntity
    {
        public string PartyId { get; set; }
        public string PartyName { get; set; }
    }
}
