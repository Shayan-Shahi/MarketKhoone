using MarketKhoone.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace MarketKhoone.Entities.Identity
{
    public class Role : IdentityRole<long>, IAuditableEntity
    {
        public Role(string name) : base(name)
        {

        }

        public Role(string name, string description) : this(name)
        {
            Description = description;
        }

        public string Description { get; set; }
        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
