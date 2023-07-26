using MarketKhoone.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace MarketKhoone.Entities.Identity
{
    public class UserClaim : IdentityUserClaim<long>, IAuditableEntity
    {
        public virtual User User { get; set; }
    }
}
