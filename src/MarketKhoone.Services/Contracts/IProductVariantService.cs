using MarketKhoone.Entities;
using MarketKhoone.ViewModels.ProductVariants;

namespace MarketKhoone.Services.Contracts
{
    public interface IProductVariantService : IGenericService<ProductVariant>
    {
        Task<int> GetProductsSellingNow();
        /// <summary>
        /// گرفتن آیدی وکد تنوع در جدول تنوع های محصول برای یخش
        /// بک اند ساخت محموله
        /// </summary>
        /// <param name="variantCodes"></param>
        /// <returns></returns>
        Task<List<GetProductVariantInCreateConsignmentViewModel>> GetProductVariantsForCreateConsignment(List<int> variantCodes);
        /// <summary>
        /// گرفتن اطلاعات مربوط به تنوع محصول
        /// جهت استفاده در ضفحه ایجاد محموله
        /// </summary>
        /// <param name="variantCode"></param>
        /// <returns></returns>
        Task<ShowProductVariantInCreateConsignmentViewModel> GetProductVariantForCreateConsignment(int variantCode);

        Task<List<ShowProductVariantViewModel>> GetProductVariants(long productId);
        Task<EditProductVariantViewModel> GetDataForEdit(long productVariantId);
        Task<ProductVariant> GetForEdit(long id);
        Task<AddEditDiscountViewModel> GetDataForAddEditDiscount(long id);
        Task<List<ProductVariant>> GetProductVariantsToAddCount(List<long> productVariantIds);
        /// <summary>
        /// برری میکنیم که آیا این تنوع برای این فروشنده از قبل اضافه شده است یا نه؟
        /// اگر اضافه شده باشد نباید اجازه بدهیم که یکبار دیگر این تنوع اضافه بشه
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<bool> IsThisVariantAddedForSeller(long? variantId, long productId);
        /// <summary>
        /// گرفتن آخرین ئکد تنوع به اضافه یک
        /// جهت استفاده در صفحه افزودن مححصول
        /// </summary>
        /// <returns></returns>
        Task<int> GetVariantCodeForCreateProductVariant();

        /// <summary>
        /// برای مثال این دسته بندی 3 رنگ دارد
        /// از کدام یک از این رنگ ها در بخش تنع محصولات استفاده شده است
        /// آیدی اون تنوع ها رو برگشت میزنیم
        ///  که به ادیمن اجازه ندیم که اون تنوع ها رو بتونه از این  دسته بندی حذف کنه
        /// </summary>
        /// <param name="selectedVariants"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<List<long>> GetAddedVariantToProductVariants(List<long> selectedVariants, long categoryId);
    }
}

