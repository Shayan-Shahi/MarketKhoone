using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Carts;
using MarketKhoone.ViewModels.Products;

namespace Marketkhoone.Web.Mappings
{
    public class CartMappingProfile :Profile
    {
        public CartMappingProfile()
        {
            #region Parameters

            DateTime now = default;

            #endregion

            this.CreateMap<Cart, ProductVariantInCartForProductInfoViewModel>();
            this.CreateMap<Cart, ShowCartInDropDownViewModel>()
                .ForMember(dest => dest.IsDiscountActive,
                    options =>
                        options.MapFrom(src => src.ProductVariant.OffPercentage != null &&
                                               (src.ProductVariant.StartDateTime <= now &&
                                                src.ProductVariant.EndDateTime >= now)))


                .ForMember(dest => dest.ProductPicture,
                    options =>
                        options.MapFrom(src => src.ProductVariant.Product.ProductMedia.First().FileName));



            this.CreateMap<Cart, ShowCartInCartPageViewModel>()
                .ForMember(dest => dest.IsDiscountActive,
                    options =>
                        options.MapFrom(src =>
                            src.ProductVariant.OffPercentage != null &&
                            (src.ProductVariant.StartDateTime <= now && src.ProductVariant.EndDateTime >= now)))

                .ForMember(dest => dest.ProductPicture,
                    options =>
                        options.MapFrom(src => src.ProductVariant.Product.ProductMedia.First().FileName))

                .ForMember(dest => dest.ProductVariantCount,
                    options =>
                        options.MapFrom(src =>
                            src.ProductVariant.Count > 3 ? (byte)0 : (byte)src.ProductVariant.Count));


            this.CreateMap<Cart, ShowCartInCheckoutPageViewModel>()
                .ForMember(dest => dest.IsDiscountActive,
                    options =>
                        options.MapFrom(src =>
                            src.ProductVariant.OffPercentage != null &&
                            (src.ProductVariant.StartDateTime <= now && src.ProductVariant.EndDateTime >= now)))

                .ForMember(dest => dest.IsDiscountActive,
                    options =>
                        options.MapFrom(src =>
                            src.ProductVariant.OffPercentage != null &&
                            (src.ProductVariant.StartDateTime <= now && src.ProductVariant.EndDateTime >= now)))

                .ForMember(dest => dest.IsDiscountActive,
                    options =>
                        options.MapFrom(src =>
                            src.ProductVariant.OffPercentage != null &&
                            (src.ProductVariant.StartDateTime <= now && src.ProductVariant.EndDateTime >= now)));



        }
    }
}
