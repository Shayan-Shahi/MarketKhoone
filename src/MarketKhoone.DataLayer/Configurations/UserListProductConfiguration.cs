using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class UserListProductConfiguration : IEntityTypeConfiguration<UserListProduct>
    {
        public void Configure(EntityTypeBuilder<UserListProduct> builder)
        {
            builder.HasKey(x => new { x.UserListId, x.ProductId });

            builder.HasOne(x => x.UserList)
                .WithMany(x => x.UserListsProducts)
                .HasForeignKey(x => x.UserListId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
