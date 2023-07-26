using MarketKhoone.Entities;
using MarketKhoone.ViewModels.DiscountCodes;

namespace MarketKhoone.Services.Contracts
{
    public interface IDiscountCodeService : IGenericService<DiscountCode>
    {
        /// <summary>
        /// چک کردن کد تخفیف برای استفاده کردن
        /// در صفحه پرداخت و بخش پست، بخش پست یعنی زمانی که فرم ارسال می شود
        /// </summary>
        /// <param name="o"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        Task<CheckDiscountCodeForPaymentViewModel> CheckForDiscountPriceForPayment(GetDiscountCodeDataViewModel model, bool showDiscountCodeId);


    }
}
