﻿using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class SellerConfiguration : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {

            builder.HasOne(x => x.User)
                .WithOne(x => x.Seller)
                .HasForeignKey<Seller>(x => x.UserId);


            builder.HasMany(x => x.ProductVariants)
                .WithOne(x => x.Seller)
                .HasForeignKey(x => x.SellerId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
