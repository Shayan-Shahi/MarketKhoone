using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Variants;

namespace MarketKhoone.Services.Contracts
{
    public interface IVariantService : IGenericService<Variant>
    {
        Task<ShowVariantsViewModel> GetVariants(ShowVariantsViewModel model);
        /// <summary>
        /// استفاده شده در صفحه شما هم فروشنده شوید
        /// این متود سه کار ار انجام میدهد
        /// آیا محثولی که وارد کرده وجود داره؟
        /// آیا تنوعی که وارد کرده وجود داره/
        /// آگه تع وارد شده رنگ است
        /// باید تنوع محصول هم رنگ باشد
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variantId"></param>
        /// <returns></returns>
        Task<(bool isISuccessful, bool IsVariantNull)> CheckProductAndVariantTypeForAddVariant(long productId, long variantId);

        /// <summary>
        /// گرفتن لیست تمامی تنوع ها
        /// استفاده شده در صفحه ویرایش تنوع دسته بندی بخش ادمین
        /// </summary>
        /// <param name="isVariantTypeColor"></param>
        /// <returns></returns>
        Task<List<ShowVariantInEditCategoryVariantViewModel>> GetVariantsForEditCategoryVariants(bool isVariantTypeColor);

        /// <summary>
        /// آیا تنوعی که قراره برای این دسته بندی اضافه شه
        /// آیدیشون به درستی وارد شده
        /// و اگر تنوع این دسته بندی رنگ است
        /// باید نوسط ادمین فقط رنگ به سمت سرور اومده باشد
        /// </summary>
        /// <param name="categoryVariantsIds"></param>
        /// <param name="categoryIsVariantColor"></param>
        /// <returns></returns>
        Task<bool> CheckVariantsCountAndConfirmStatusForEditCategoryVariants(List<long> categoryVariantsIds, bool categoryIsVariantColor);
    }
}
