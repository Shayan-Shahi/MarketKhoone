using MarketKhoone.Entities;

namespace MarketKhoone.Services.Contracts
{
    public interface ICategoryBrandService : ICustomGenericService<CategoryBrand>
    {
        Task<bool> CheckCategoryBrand(long categoryId, long brandId);
        /// <summary>
        /// گرفتن میزان درصد کمیسیون یک برند و دسته بندی
        /// حهت ساتفاده در صفحه ایجاد محصول
        /// هنگام عوض شدن سلکت باکس، این متود فراخوانی میشود
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="brandId"></param>
        /// <returns></returns>
        Task<(bool isSuccessful, byte value)> GetCommissionPercentage(long categoryId, long brandId);
    }
}
