using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class CategoryVariantConfiguration : IEntityTypeConfiguration<CategoryVariant>
    {
        public void Configure(EntityTypeBuilder<CategoryVariant> builder)
        {
            builder.HasKey(x => new { x.CategoryId, x.VariantId });
        }
    }
}
