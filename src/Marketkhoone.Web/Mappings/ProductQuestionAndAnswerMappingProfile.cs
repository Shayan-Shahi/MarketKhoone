using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace Marketkhoone.Web.Mappings
{
    public class ProductQuestionAndAnswerMappingProfile : Profile
    {
        public ProductQuestionAndAnswerMappingProfile()
        {
            this.CreateMap<ProductQuestionAndAnswer, ProductQuestionForProductInfoViewModel>();
            this.CreateMap<ProductQuestionAndAnswer, ProductQuestionAnswerForProductInfoViewModel>()
                .ForMember(dest => dest.Like,
                    options =>
                        options.MapFrom(src =>
                            src.ProductsQuestionsAnswersScores.LongCount(x => x.IsLike)))


                .ForMember(dest => dest.Dislike,
                    options =>
                        options.MapFrom(src => src.ProductsQuestionsAnswersScores.LongCount(x => !x.IsLike)))

                .ForMember(dest=>dest.Name,
                    options=>
                        options.MapFrom(src=>src.IsUnknown ? null
                            :(
                            src.UserId !=null ? src.User.FullName : src.Seller.ShopName)))


                .ForMember(dest=>dest.IsShop,
                    options=>
                        options.MapFrom(src=>src.SellerId !=null))

                ;

                
        }
    }
}
