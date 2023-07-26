using MarketKhoone.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace MarketKhoone.Entities.Identity
{
    public class UserRole : IdentityUserRole<long>, IAuditableEntity
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
