using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace Marketkhoone.Web.Mappings
{
    public class AnswerScoreMappingProfile : Profile
    {
        public AnswerScoreMappingProfile()
        {
            this.CreateMap<ProductQuestionAnswerScore, LikedAnswerByUserViewModel>();
        }
    }
}
