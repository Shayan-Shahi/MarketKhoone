using MarketKhoone.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace MarketKhoone.Entities.Identity
{
    public class RoleClaim : IdentityRoleClaim<long>, IAuditableEntity
    {
        public virtual Role Role { get; set; }
    }
}
