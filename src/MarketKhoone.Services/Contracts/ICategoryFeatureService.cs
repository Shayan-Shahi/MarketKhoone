using MarketKhoone.Entities;
using MarketKhoone.ViewModels.CategoryFeatures;

namespace MarketKhoone.Services.Contracts
{
    public interface ICategoryFeatureService : ICustomGenericService<CategoryFeature>
    {

        Task<CategoryFeature> GetCategoryFeature(long categoryId, long featureId);
        Task<List<CategoryFeatureForCreateProductViewModel>> GetCategoryFeatures(long categoryId);
        Task<bool> CheckCategoryFeature(long categoryId, long featureId);
        Task<bool> CheckCategoryFeatureCount(long categoryId, List<long> featureIds);
    }
}

