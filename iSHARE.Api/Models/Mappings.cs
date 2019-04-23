using iSHARE.Identity;

namespace iSHARE.Api.Models
{
    public static class Mappings
    {
        public static UserOverviewViewModel Map(this UserModel entity)
        {
            return new UserOverviewViewModel
            {
                Id = entity.Id,
                IdentityId = entity.IdentityId,
                Username = entity.Username,
                PartyId = entity.PartyId,
                PartyName = entity.PartyName,
                CreatedDate = entity.CreatedDate,
                Roles = entity.Roles,
                Active = entity.Active
            };
        }
    }
}
