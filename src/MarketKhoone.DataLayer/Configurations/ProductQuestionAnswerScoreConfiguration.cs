using MarketKhoone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketKhoone.DataLayer.Configurations
{
    public class ProductQuestionAnswerScoreConfiguration : IEntityTypeConfiguration<ProductQuestionAnswerScore>
    {
        public void Configure(EntityTypeBuilder<ProductQuestionAnswerScore> builder)
        {
            builder.HasKey(x => new { x.UserId, x.AnswerId });

            builder.HasOne(x => x.User)
                .WithMany(x => x.ProductQuestionAnswerScores)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
