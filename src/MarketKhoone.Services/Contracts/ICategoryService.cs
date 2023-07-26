using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Categories;
using MarketKhoone.ViewModels.Search;

namespace MarketKhoone.Services.Contracts
{
    public interface ICategoryService : IGenericService<Category>
    {
        Task<ShowCategoriesViewModel> GetCategories(ShowCategoriesViewModel model);

        Task<Dictionary<long, string>> GetCategoriesToShowInSelectBoxAsync(long? id = null);
        Task<EditCategoryViewModel> GetForEdit(long id);
        Task<List<List<ShowCategoryForCreateProductViewModel>>> GetCategoriesForCreateProduct(long[] selectedCategoriesIds);

        Task<List<string>> GetCategoryBrands(long selectedCategoryId);
        Task<Category> GetCategoryWithItsBrands(long selectedCategoryId);
        Task<bool> CanAddFakeProduct(long categoryId);
        Task<Dictionary<long, string>> GetCategoriesWithNoChild();
        Task<Dictionary<long, string>> GetLastCategoriesWithNoChild();

        /// <summary>
        /// بررسی کد محصولات در صفحه مقایشه محصولات
        /// باید دسته بندی تمامی محصولات وارد شده مشابه با اولین محصول وارد شده باشد
        /// برای مثال اگر دسته بندی اولین محصول گوشی موبایل یاشد
        /// سایر محصولات هم باید در دسته بندی گوشی موبایل باشند
        /// </summary>
        /// <param name="productCodes"></param>
        /// <returns></returns>
        Task<bool> CheckProductCategoryIdsInComparePage(params int[] productCodes);
        /// <summary>
        /// فروشنده مورد نظر از چه دسته بندی هایی استفاده کرده است
        /// آنها را برگشت میزنیم
        ///   در ادمین جهت استفاده در صفحه مدیریت محصول در بخش پنل فروشندگان
        /// </summary>
        Task<Dictionary<long, string>> GetSellerCategories();

        // کالای دیجیتال=>گوشی موبایل=>اس هفت
        //کاربر در تب اول ایجاد محصول دسته بندی گوشی موبایل رو انتخاب کرده و قضد
        //داره یه محصول اس هفت ایجاد کنه
        //ما لازم داریم کالای دیجیتال رو بدست بیاریم یعنی برگرد عقب تا زمانیکه
        //پرنت آیدی نال میشه، اون دسته بندی ماست
        Task<(bool isSuccessful, List<long> categoryIds)> GetCategoryParentIds(long categoryId);
        /// <summary>
        /// اظلاعات صفح جستجو دسته بندی ها
        /// </summary>
        /// <param name="categorySlug"></param>
        /// <param name="brandSlug"></param>
        /// <returns></returns>
        Task<SearchOnCategoryViewModel> GetSearchOnCategoryData(string categorySlug, string brandSlug);

        Task<List<string>> GetCategoryVariants(long selectedCategoryId);

        //آیا این تنوع دسته بندی رنگ است یا اندازه یا کلا تنوع ندارد
        //در ضمن اگه رنگه چه رنگی؟ اگه سازه چه سایزی؟
        // استفاده شده در صحه ویرایش تنوع دسته بندی
        Task<bool?> IsVariantTypeColor(long categoryId);

        /// <summary>
        /// خواند یک دسته بندی
        // تمامی رکورد های تنوع این دسته بندی رو هم اینکلود میکنیم
        //استفاده شده در صفحه ویرایش تنوع دسته بندی بخش ادمین
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<Category> GetCategoryForEditVariant(long categoryId);
    }
}

