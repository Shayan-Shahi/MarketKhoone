using EShopMarket.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Carts;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Cart
{
    public class IndexModel : PageBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IUnitOfWork _uow;
        private readonly IViewRendererService _viewRendererService;

        public IndexModel(IFacadeServices facadeServices, IUnitOfWork uow, IViewRendererService viewRendererService)
        {
            _facadeServices = facadeServices;
            _uow = uow;
            _viewRendererService = viewRendererService;
        }

        #endregion

        public List<ShowCartInCartPageViewModel> CartItems { get; set; }
        public async Task OnGet()
        {
            var userId = User.Identity.GetLoggedInUserId();
            CartItems = await _facadeServices.CartService.GetCartsForCartPage(userId);

        }

        public async Task<IActionResult> OnPostAddProductVariantToCart(long productVariantId, bool isInCrease)
        {
            var productVariant = await _facadeServices.ProductVariantService.FindByIdAsync(productVariantId);
            if (productVariant is null)
            {
                return Json(new JsonResultOperation(false));
            }

            var userId = User.Identity.GetLoggedInUserId();

            var cart = await _facadeServices.CartService.FindAsync(userId, productVariantId);

            if (cart is null)
            {
                var cartToAdd = new MarketKhoone.Entities.Cart()
                {
                    ProductVariantId = productVariantId,
                    UserId = userId,
                    Count = 1
                };
                await _facadeServices.CartService.AddAsync(cartToAdd);
            }
            else if (isInCrease)
            {

                //فروشنده تعیین کرده حداکث تعدادی که کاربر طی هر خرد میتونه 
                // زا یان محصول وارد سبد خرید کنه و خریدشو انجام بده 3 مورد است

                // مقدار داخل سبد خرید قبل فشردن دکمه به علاوه
                // 3
                cart.Count++;
                //بعد از زدن دکمه به علاوه 
                //4

                //چون مقدار داخل سبد خرید بیتر از مقداری است که فروشنده تعیین کرده
                // در نتیجه مقدار داخل سبد خرید رو به مقدار تعیین شده توسط فروشندده تغییر میدهیم
                if (cart.Count > productVariant.MaxCountInCart)
                    cart.Count = productVariant.MaxCountInCart;

                //موجودی انبار 2 عدد است
                // موجودی داخل سبد خرید هم 2 عدد است
                // حالا روی دکمخ به علاوه کلیک میشه
                //چون حداکثر تعداد یکه فروشنده تعیین کرده 3 است
                //در نتیجه از ایف بالا عبور میکنهو به ایف پایین میرسه
                //و چون 3 بزرگتر از موجودی انبار یعنی 2 است
                //در نتیجه مقدار داخل سبد خرید هم به 2 تغییر میکنه

                if (cart.Count > productVariant.Count)
                    cart.Count = (short)productVariant.Count;
            }
            else
            {
                cart.Count--;
                if (cart.Count == 0)
                {
                    _facadeServices.CartService.Remove(cart);
                }
            }

            await _uow.SaveChangesAsync();

            var carts = await _facadeServices.CartService.GetCartsForCartPage(userId);

            var cartBody = string.Empty;

            ///اگر مورد ی داخل سبد خرید نبودف "پارشل سبد خرید خالی" رو نشون میدیم
            if (carts.Count == 0)
            {
                cartBody = await _viewRendererService.RenderViewToStringAsync("~/Pages/Cart/_EmptyCartPartial.cshtml");
            }
            else
            {
                cartBody = await _viewRendererService.RenderViewToStringAsync("~/Pages/Cart/_CartBodyPartial.cshtml",
                    carts);
            }

            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = new
                {
                    CartBody = cartBody
                }
            });
        }

        public async Task<IActionResult> OnPostRemoveAllItemsInCart()
        {
            var userId = User.Identity.GetLoggedInUserId();

            var allItemsInCart = await _facadeServices.CartService.GetAllCartItems(userId);

            _facadeServices.CartService.RemoveRange(allItemsInCart);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = new
                {
                    CartBody = await _viewRendererService.RenderViewToStringAsync(
                        "~/Pages/Cart/_EmptyCartPartial.cshtml")
                }
            });
        }

    }
}
