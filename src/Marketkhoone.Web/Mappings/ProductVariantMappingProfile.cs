using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.ProductVariants;

namespace Marketkhoone.Web.Mappings
{
    public class ProductVariantMappingProfile : Profile
    {
        public ProductVariantMappingProfile()
        {
            this.CreateMap<EditProductVariantViewModel, ProductVariant>();
        }
    }
}
