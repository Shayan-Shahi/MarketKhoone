using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace Marketkhoone.Web.Mappings
{
    public class ProductMediaMappingProfile : Profile
    {
        public ProductMediaMappingProfile()
        {
            this.CreateMap<ProductMedia, ProductMediaForProductDetailsViewModel>();
            this.CreateMap<ProductMedia, ProductMediaForProductInfoViewModel>();
        }
    }
}
