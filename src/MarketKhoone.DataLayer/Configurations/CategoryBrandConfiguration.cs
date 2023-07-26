using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class CategoryBrandConfiguration : IEntityTypeConfiguration<CategoryBrand>
    {
        public void Configure(EntityTypeBuilder<CategoryBrand> builder)
        {
            builder.HasKey(x => new { x.CategoryId, x.BrandId });
        }
    }
}
