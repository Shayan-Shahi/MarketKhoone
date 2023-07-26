using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace Marketkhoone.Web.Mappings
{
    public class ProductFeatureMappingProfile : Profile
    {
        public ProductFeatureMappingProfile()
        {
            this.CreateMap<ProductFeature, ShowFeatureInCompareViewModel>();
        }
    }
}
