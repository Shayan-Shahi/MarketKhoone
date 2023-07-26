using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace Marketkhoone.Web.Mappings
{
    public class ParcelPostItemMappingProfile : Profile
    {
        public ParcelPostItemMappingProfile()
        {
            this.CreateMap<IEnumerable<ParcelPostItem>, ShowProductInProfileCommentViewModel>()
                .ForMember(dest => dest.Picture,
                    options =>
                        options.MapFrom(src => src.First().ProductVariant.Product.ProductMedia.First().FileName))

                .ForMember(dest => dest.Title,
                    options =>
                        options.MapFrom(src => src.First().ProductVariant.Product.PersianTitle));
        }
    }
}
