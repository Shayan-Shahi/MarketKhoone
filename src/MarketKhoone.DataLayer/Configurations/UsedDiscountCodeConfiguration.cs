using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class UsedDiscountCodeConfiguration : IEntityTypeConfiguration<UsedDiscountCode>
    {
        public void Configure(EntityTypeBuilder<UsedDiscountCode> builder)
        {
            builder.HasKey(x => new { x.UserId, x.DiscountCodeId, x.OrderId });

            builder.HasOne(x => x.User)
                .WithMany(x => x.UsedDiscountCodes)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
