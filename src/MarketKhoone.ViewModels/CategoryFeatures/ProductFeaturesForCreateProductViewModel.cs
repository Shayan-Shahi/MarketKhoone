using MarketKhoone.ViewModels.FeatureConstantValues;

namespace MarketKhoone.ViewModels.CategoryFeatures;

public class ProductFeaturesForCreateProductViewModel
{
    public List<CategoryFeatureForCreateProductViewModel> Features { get; set; }
    public List<ShowCategoryFeatureConstantValueViewModel> FeaturesConstantValues { get; set; }
}