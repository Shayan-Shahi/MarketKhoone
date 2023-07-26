using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class CommentScoreConfiguration : IEntityTypeConfiguration<CommentScore>
    {
        public void Configure(EntityTypeBuilder<CommentScore> builder)
        {
            builder.HasKey(x => new { x.UserId, x.ProductCommentId });

            builder.HasOne(x => x.User)
                .WithMany(x => x.CommentScores)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
