using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Carts;
using MarketKhoone.ViewModels.Products;

namespace MarketKhoone.Services.Contracts
{
    public interface ICartService : ICustomGenericService<Cart>
    {
        /// <summary>
        /// استفاده شده در صفحه تکی محصول
        /// از داخل این تنوع ها  که به متد پاس داده شده
        /// کدامیک برای این کاربر داخل سبد خریدش اضافه شده است
        /// </summary>
        /// <param name="productVariantsIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ProductVariantInCartForProductInfoViewModel>> GetProductVariantsInCart(List<long> productVariantsIds, long userId);

        /// <summary>
        /// استفاده شده در لایوت اصلی و در داپ داون سبد خرید
        /// گرفتن محصولاتی که این کاربر وارد سبد خرید کرده است؟
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ShowCartInDropDownViewModel>> GetCartsForDropDown(long userId);

        Task<List<ShowCartInCartPageViewModel>> GetCartsForCartPage(long userId);
        Task<List<Cart>> GetAllCartItems(long userId);
        Task<List<ShowCartInCheckoutPageViewModel>> GetCartsForCheckoutPage(long userId);
        Task<List<ShowCartInPaymentPageViewModel>> GetCartsForPaymentPage(long userId);
        /// <summary>
        /// گرفتن محصولات داخل سبد خرید این کاربر
        /// برای بخش ایجاد سفارش
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ShowCartForCreateOrderAndPayViewModel>> GetCartForCreateOrderAndPay(long userId);
    }
}
