using MarketKhoone.Entities;

namespace MarketKhoone.Services.Contracts
{
    public interface ICategoryVariantService : ICustomGenericService<CategoryVariant>
    {
        /// <summary>
        /// این دسته بندی چه تنوع هایی دارد؟
        /// استفاده شده در صفحه ویرایش تنوع دسته بندی بحش ادمین
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<List<long>> GetCategoryVariants(long categoryId);
    }
}
