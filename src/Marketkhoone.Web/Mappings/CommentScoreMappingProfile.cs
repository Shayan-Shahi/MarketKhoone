using AutoMapper;
using MarketKhoone.ViewModels.Products;

namespace Marketkhoone.Web.Mappings
{
    public class CommentScoreMappingProfile : Profile
    {
        public CommentScoreMappingProfile()
        {
            this.CreateMap<CommentScoreMappingProfile, LikedCommentByUserViewModel>();
        }
    }
}
