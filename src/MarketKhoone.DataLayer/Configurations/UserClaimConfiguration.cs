﻿using MarketKhoone.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasOne(userClaim => userClaim.User)
                .WithMany(userClaim => userClaim.UserClaims)
                .HasForeignKey(userClaim => userClaim.UserId);

            builder.ToTable("UserClaims");
        }
    }
}
