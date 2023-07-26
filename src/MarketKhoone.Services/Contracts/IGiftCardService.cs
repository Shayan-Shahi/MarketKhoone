using MarketKhoone.Entities;
using MarketKhoone.ViewModels.GiftCards;

namespace MarketKhoone.Services.Contracts
{
    public interface IGiftCardService : ICustomGenericService<GiftCard>

    {
        /// <summary>
        /// چک کردن کارت هدیه برای استفاده کردن در صفحه پرداخت و بخش پست
        /// بهش پست یعنی زمانی که فرم ارسال می شود
        /// </summary>
        /// <param name="model"></param>
        /// <param name="showGiftCardId"></param>
        /// <returns></returns>
        Task<CheckGiftCardCodeForPaymentViewModel> CheckForGiftCardPriceForPayment(GetGiftCardCodeDataViewModel model, bool showGiftCardId);
    }
}
