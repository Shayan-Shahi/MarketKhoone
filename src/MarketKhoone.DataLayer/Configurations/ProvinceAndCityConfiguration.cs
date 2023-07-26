using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class ProvinceAndCityConfiguration : IEntityTypeConfiguration<ProvinceAndCity>
    {
        public void Configure(EntityTypeBuilder<ProvinceAndCity> builder)
        {
            builder.HasMany(x => x.SellerProvinces)
                .WithOne(x => x.Province)
                .HasForeignKey(x => x.ProvinceId);

            builder.HasMany(x => x.SellerCities)
                .WithOne(x => x.City)
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);


            #region Address

            builder.HasMany(x => x.AddressProvinces)
                .WithOne(x => x.Province)
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.AddressCities)
                .WithOne(x => x.City)
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion
        }
    }
}
