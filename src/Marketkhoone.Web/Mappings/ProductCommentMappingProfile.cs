using AutoMapper;
using DNTPersianUtils.Core;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.ProductComments;
using MarketKhoone.ViewModels.Products;

namespace Marketkhoone.Web.Mappings
{
    public class ProductCommentMappingProfile : Profile
    {
        public ProductCommentMappingProfile()
        {
            this.CreateMap<ProductComment, ProductCommentForProductInfoViewModel>()
                .ForMember(dest => dest.CreatedDateTime,
                    options =>
                        options.MapFrom(src => src.CreatedDateTime.ToLongPersianDateTime()))


                .ForMember(dest => dest.Like,
                    options =>
                        options.MapFrom(src => src.CommentsScores.LongCount(x => x.IsLike)))


                .ForMember(dest => dest.Dislike,
                    options =>
                        options.MapFrom(src => src.CommentsScores.LongCount(x => !x.IsLike)))

                .ForMember(dest => dest.Name,
                    options =>
                        options.MapFrom(src =>
                            src.IsUnknown ? null : (src.UserId != null ? src.User.FullName : src.Seller.ShopName)))

                .ForMember(dest => dest.IsShop,
                    options =>
                        options.MapFrom(src => src.SellerId != null));


            this.CreateMap<ProductComment, ShowProductCommentInProfile>()
                .ForMember(dest => dest.MainPicture,
                    options =>
                        options.MapFrom(src => src.Product.ProductMedia.First().FileName))

                .ForMember(dest => dest.IsSeller,
                    options =>
                        options.MapFrom(src => src.SellerId != null))
                .ForMember(dest=>dest.Like,
                    options=>
                        options.MapFrom(src=>src.CommentsScores.LongCount(x=>x.IsLike)));

        

        }
    }
}
