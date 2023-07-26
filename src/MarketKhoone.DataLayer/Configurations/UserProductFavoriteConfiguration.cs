﻿using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class UserProductFavoriteConfiguration : IEntityTypeConfiguration<UserProductFavorite>
    {
        public void Configure(EntityTypeBuilder<UserProductFavorite> builder)
        {
            builder.HasKey(x => new { x.UserId, x.ProductId });


            builder.HasOne(x => x.User)
                .WithMany(x => x.UserProductFavorites)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
