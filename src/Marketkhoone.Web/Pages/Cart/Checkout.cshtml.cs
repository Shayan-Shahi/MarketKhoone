using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Carts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketkhoone.Web.Pages.Cart
{
    //[Authorize]
    public class CheckoutModel : PageModel
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;

        public CheckoutModel(IFacadeServices facadeServices)
        {
            _facadeServices = facadeServices;
        }

        #endregion
        public CheckoutViewModel CheckoutPage { get; set; } = new();
        public async Task<IActionResult> OnGet()
        {

            var userId = User.Identity.GetLoggedInUserId();

            CheckoutPage.CartItems = await _facadeServices.CartService.GetCartsForCheckoutPage(userId);

            //اگر سبد خرید خالی بود، کاربر رو به صفحه سبد خرید انتقال بده
            if (CheckoutPage.CartItems.Count < 1)
            {
                return RedirectToPage("Index");
            }

            CheckoutPage.UserAddress = await _facadeServices.AddressService.GetAddressForCheckoutPage(userId);
            return Page();

        }
    }
}
