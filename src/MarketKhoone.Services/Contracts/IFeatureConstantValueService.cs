using MarketKhoone.Entities;
using MarketKhoone.ViewModels.FeatureConstantValues;

namespace MarketKhoone.Services.Contracts
{
    public interface IFeatureConstantValueService : IGenericService<FeatureConstantValue>
    {
        Task<ShowFeatureConstantValuesViewModel> GetFeatureConstantValues(ShowFeatureConstantValuesViewModel model);
        Task<bool> CheckNonConstantValue(long categoryId, List<long> featureIds);
        /// <summary>
        /// آیا به همان تعداد که مقادیر ثابت وجود دارد
        /// به همان میزان هم مقادیر ثابت توسط فروشنده به سمت سرور آمده است؟
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="featureConstantValueIds"></param>
        /// <returns></returns>
        Task<bool> CheckConstantValue(long categoryId, List<long> featureConstantValueIds);

        Task<List<FeatureConstantValueForCreateProductViewModel>> GetFeatureConstantValuesForCreateProduct(long categoryId);
        Task<List<ShowCategoryFeatureConstantValueViewModel>> GetFeatureConstantValuesByCategoryId(long categoryId);
    }
}
