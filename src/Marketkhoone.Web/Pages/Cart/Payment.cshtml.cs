using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Enums.Order;
using MarketKhoone.Entities.Enums.ParcelPostEnums;
using MarketKhoone.Entities.Enums.Product;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Carts;
using MarketKhoone.ViewModels.DiscountCodes;
using MarketKhoone.ViewModels.GiftCards;
using Microsoft.AspNetCore.Mvc;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Gateway.ZarinPal;

namespace Marketkhoone.Web.Pages.Cart
{
    //[Authorize]
    public class PaymentModel : PageBase
    {

        #region Constructor
        private readonly IFacadeServices _facadeService;
        private readonly IOnlinePayment _onlineService;
        private readonly IUnitOfWork _uow;

        public PaymentModel(IFacadeServices facadeService
          , IOnlinePayment onlineService, IUnitOfWork uow)
        {
            _facadeService = facadeService;
            _onlineService = onlineService;
            _uow = uow;
        }
        #endregion

        public PaymentViewModel PaymentPage { get; set; } = new();

        [BindProperty]
        public CreateOrderAndPayViewModel CreateOrderAndPayModel { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var userId = User.Identity.GetLoggedInUserId();
            PaymentPage.CartItems = await _facadeService.CartService.GetCartsForPaymentPage(userId);

            //اگر سبد خرید خالی بود، کاربر را به صفحه سبد خرید انتقال بده
            if (PaymentPage.CartItems.Count < 1)
            {
                return RedirectToPage("Index");
            }

            return Page();
        }
        /// <summary>
        /// ایجاد سفارش و انتقال کاربر به درگاه بانکی
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPost()
        {
            var userId = User.Identity.GetLoggedInUserId();

            if (!ModelState.IsValid)
            {
                PaymentPage.CartItems = await _facadeService.CartService.GetCartsForPaymentPage(userId);


                //اگر سبد خرید خالی بود، کاربر رو به صقحه سبد خرید انتقال بده
                if (PaymentPage.CartItems.Count < 1)
                {
                    return RedirectToPage("Index");
                }

                return Page();
            }

            //پرداخل سفارش از کیف پول کاربر
            if (CreateOrderAndPayModel.PayFormWallet)
            {
                // todo: pay order price form wallet
            }

            var address = await _facadeService.AddressService.GetAddressForCreateOrderAndyPay(userId);

            //آیا کاربر آدرس داره؟
            if (!address.hasUserAddress)
            {
                return RedirectToPage("CheckOut");
            }

            var orderToAdd = new MarketKhoone.Entities.Order()
            {
                UserId = userId,
                AddressId = address.AddressId,
                PayFromWallet = false,
                Status = OrderStatus.WaitingForPaying
            };

            //محصولات داخل سبد خرید

            var cartItems = await _facadeService.CartService.GetCartForCreateOrderAndPay(userId);
            if (cartItems.Count < 1)
            {
                return RedirectToPage("Index");
            }

            //ارسال عادی

            var normalProducts = cartItems
                .Where(x => x.ProductVariantProductDimension == Dimension.Normal).ToList();
            //ارسال کالاهای سنگین
            var heavyProducts = cartItems
                .Where(x => x.ProductVariantProductDimension == Dimension.Heavy).ToList();

            //ارشال کالاهای فوق سنگین
            var ultraHeavyProducts = cartItems
                .Where(x => x.ProductVariantProductDimension == Dimension.UltraHeavy).ToList();

            //قیمت کل کالاهایی که ابعادشان عادی است
            var sumPriceOfNormalProducts = normalProducts
                .Sum(x => (x.IsDiscountActive ? x.ProductVariantOffPrice.Value : x.ProductVariantPrice)
                *
                x.Count);

            if (normalProducts.Count > 0)
            {
                //مرسوله
                var parcelPostToAdd = new MarketKhoone.Entities.ParcelPost()
                {
                    Dimension = Dimension.Normal,
                    Status = ParcelPostStatus.WaitingForPaying,
                    ShippingPrice = sumPriceOfNormalProducts < 500000 ? 30000 : 0
                };

                //محتوای داخل مرسوله
                foreach (var normalProduct in normalProducts)
                {
                    var parcelPostItemToAdd = new MarketKhoone.Entities.ParcelPostItem()
                    {
                        Count = normalProduct.Count,
                        ProductVariantId = normalProduct.ProductVariantId,
                        GuaranteeId = normalProduct.ProductVariantGuaranteeId,
                        Score = normalProduct.Score,
                        Price = normalProduct.ProductVariantPrice,
                        Order = orderToAdd
                    };

                    if (normalProduct.IsDiscountActive)
                        parcelPostItemToAdd.DiscountPrice =
                            normalProduct.ProductVariantPrice - normalProduct.ProductVariantOffPrice.Value;
                    parcelPostToAdd.ParcelPostItems.Add(parcelPostItemToAdd);
                }
                orderToAdd.ParcelPosts.Add(parcelPostToAdd);
            }

            if (heavyProducts.Count > 0)
            {
                //مرسوله
                var parcelPostToAdd = new MarketKhoone.Entities.ParcelPost()
                {
                    Dimension = Dimension.UltraHeavy,
                    Status = ParcelPostStatus.WaitingForPaying,
                    ShippingPrice = 0
                };

                //محتوای داخل مرسوله

                foreach (var ultraHeavyProduct in ultraHeavyProducts)
                {
                    var parcelPostItemToAdd = new MarketKhoone.Entities.ParcelPostItem()
                    {
                        Count = ultraHeavyProduct.Count,
                        ProductVariantId = ultraHeavyProduct.ProductVariantId,
                        GuaranteeId = ultraHeavyProduct.ProductVariantGuaranteeId,
                        Score = ultraHeavyProduct.Score,
                        Price = ultraHeavyProduct.ProductVariantPrice,
                        Order = orderToAdd
                    };

                    if (ultraHeavyProduct.IsDiscountActive)
                        parcelPostItemToAdd.DiscountPrice =
                            ultraHeavyProduct.ProductVariantPrice - ultraHeavyProduct.ProductVariantOffPrice.Value;
                    parcelPostToAdd.ParcelPostItems.Add(parcelPostItemToAdd);
                }
                orderToAdd.ParcelPosts.Add(parcelPostToAdd);

            }

            #region Empty user Cart items

            // باید سبد خرید کاربر رو خالی کنیم
            var cartItemsToRemove = new List<MarketKhoone.Entities.Cart>();
            foreach (var cartItem in cartItems)
            {
                cartItemsToRemove.Add(new MarketKhoone.Entities.Cart()
                {
                    ProductVariantId = cartItem.ProductVariantId,
                    UserId = userId
                });
            }

            _facadeService.CartService.RemoveRange(cartItemsToRemove);

            #endregion

            // قیمت نهایی بدون محاسیه تخفیف

            var totalPrice = cartItems.Sum(x => x.ProductVariantPrice * x.Count);

            //ثسمت نهایی محصولات اخل سبد خرید کاربر با محاسبه تخفیف انها

            var totalPriceWithDiscount = cartItems
                .Sum(x => (x.IsDiscountActive ? x.ProductVariantOffPrice.Value : x.ProductVariantPrice)
                          *
                          x.Count);

            // مجموع امتیاز هایی که کاربر بعد از پایان مهلت مرجوعی به دست میاورد
            var sumScore = cartItems.Sum(x => x.Score);
            if (sumScore > 150)
                sumScore = 150;

            //این سفارش در چند مرحله ارسال می شود
            var shippingCount = 0;

            if (normalProducts.Count > 0)
                shippingCount++;
            if (heavyProducts.Count > 0)
                shippingCount++;
            if (heavyProducts.Count > 0)
                shippingCount++;


            //مجموع قیمت حمل و نقل مرسوله ها
            var sumPriceOfShipping = 0;

            if (sumPriceOfShipping < 500000 && normalProducts.Count > 0)
            {
                sumPriceOfShipping += 30000;

            }

            if (sumPriceOfShipping < 500000 && heavyProducts.Count > 0)
            {
                sumPriceOfShipping += 45000;
            }

            // قیمت محصولات داخل سبد خرید کاربر به علاوه هزینه حمل و نقل مرسوله ها

            var finalPrice = totalPriceWithDiscount + sumPriceOfShipping;

            var discountCode = CreateOrderAndPayModel.DiscountCode;

            var discountCodePrice = 0;

            if (!string.IsNullOrWhiteSpace(discountCode))
            {
                var checkDiscountCode =
                    await _facadeService.DiscountCodeService.CheckForDiscountPriceForPayment(new(finalPrice,
                        discountCode), true);

                if (!checkDiscountCode.Result)
                {
                    PaymentPage.CartItems = await _facadeService.CartService.GetCartsForPaymentPage(userId);


                    //اگر سبد خرید کاربر خالی بود؛ کاربر رو به صفحه یبد خرید انتقال بده
                    if (PaymentPage.CartItems.Count < 1)
                    {
                        return RedirectToPage("Index");
                    }
                    ModelState.AddModelError(string.Empty, checkDiscountCode.Message);
                    return Page();
                }

                orderToAdd.DiscountCodeId = checkDiscountCode.DiscountCodeId;
                orderToAdd.DiscountCodePrice = discountCodePrice = checkDiscountCode.DiscountPrice;
            }

            finalPrice = finalPrice - discountCodePrice <= 0 ? 0 : finalPrice - discountCodePrice;


            var giftCardCode = CreateOrderAndPayModel.GiftCardCode;

            var giftCardCodePrice = 0;

            if (!string.IsNullOrWhiteSpace(giftCardCode))
            {
                if (finalPrice == 0)
                {
                    return RedirectToPage(PublicConstantStrings.Error500PageName);
                }

                var checkGiftCardCode =
                    await _facadeService.GiftCardService.CheckForGiftCardPriceForPayment(new(finalPrice, giftCardCode),
                        true);

                if (!checkGiftCardCode.Result)
                {
                    PaymentPage.CartItems = await _facadeService.CartService.GetCartsForPaymentPage(userId);

                    //اگر سبد خرید کاربر خالی بود، کاربر رو به سبد خرید انتقال بده
                    if (PaymentPage.CartItems.Count < 1)
                    {
                        return RedirectToPage("Index");
                    }
                    ModelState.AddModelError(string.Empty, checkGiftCardCode.Message);
                    return Page();
                }

                orderToAdd.ReservedGiftCardId = checkGiftCardCode.GiftCardId;
                orderToAdd.GiftCardCodePrice = giftCardCodePrice = checkGiftCardCode.DiscountPrice;
            }

            finalPrice = finalPrice - giftCardCodePrice <= 0 ? 0 : finalPrice - giftCardCodePrice;

            //کاربر بعد از درگاه به چه آدرسی هدایت شود

            var callBackUrl = Url.PageLink("VerifyPayment", null, null, Request.Scheme);

            var result = await _onlineService.RequestAsync(invoice =>
            {
                invoice
                    .SetAmount(finalPrice)
                    .SetCallbackUrl(callBackUrl)
                    .SetGateway(CreateOrderAndPayModel.PaymentGateway.ToString())
                    .UseAutoIncrementTrackingNumber();
                if (CreateOrderAndPayModel.PaymentGateway == PaymentGateway.Zarinpal)
                {
                    invoice.SetZarinPalData(new ZarinPalInvoice("No description"));
                }
            });

            orderToAdd.OrderNumber = result.TrackingNumber;
            orderToAdd.PaymentGateway = CreateOrderAndPayModel.PaymentGateway;
            orderToAdd.TotalPrice = totalPrice;
            var discountPrice = totalPrice - totalPriceWithDiscount;


            //کل تخفیفات
            // چرا نمیشه از 
            //+=
            // برای جمع بستن موارد پایین استفاده کرد؟
            //چون نمیتوان از عملگر فوق برای 
            //Int nullable
            //استفاده کرد
            //orderToAdd.DiscountPrice += discountCodePrice;

            if (discountPrice > 0)
            {
                //تخفیف خود کالاها
                orderToAdd.DiscountPrice = discountPrice;
            }

            if (discountCodePrice > 0)
            {
                //کد تخفیف
                orderToAdd.DiscountPrice = discountPrice + discountCodePrice;
            }

            if (giftCardCodePrice > 0)
            {
                //کارت هدیه
                orderToAdd.DiscountPrice = discountPrice + discountCodePrice + giftCardCodePrice;
            }

            orderToAdd.FinalPrice = finalPrice;
            orderToAdd.TotalScore = (byte)sumScore;
            orderToAdd.ShippingCount = (byte)shippingCount;


            if (finalPrice == 0)
            {
                //افزودن رکورد به جدول : کد های تخفیف استفاده شده
                if (orderToAdd.DiscountCodeId != null)
                {
                    orderToAdd.UsedDiscountCodes.Add(new()
                    {
                        UserId = userId,
                        DiscountCodeId = orderToAdd.DiscountCodeId.Value
                    });
                }

                if (orderToAdd.ReservedGiftCardId != null)
                {
                    orderToAdd.GiftCardId = orderToAdd.ReservedGiftCardId;
                    orderToAdd.ReservedGiftCardId = null;

                }
                //وضعیت مرسوله های این سفارش رو با حالت "در حال پردازش" تغییر میدهیم
                foreach (var parcelPost in orderToAdd.ParcelPosts)
                {
                    parcelPost.Status = ParcelPostStatus.Processing;
                }

                orderToAdd.Status = OrderStatus.Processing;
                orderToAdd.IsPay = true;

                await _facadeService.OrderService.AddAsync(orderToAdd);
                await _uow.SaveChangesAsync();

                return RedirectToPage("VerifyPayment", new { orderNumber = orderToAdd.OrderNumber });
            }

            if (result.IsSucceed)
            {
                await _facadeService.OrderService.AddAsync(orderToAdd);
                await _uow.SaveChangesAsync();
                return result.GatewayTransporter.TransportToGateway();
            }

            return RedirectToPage(PublicConstantStrings.Error500PageName);


        }

        public async Task<IActionResult> OnGetCheckForDiscount(GetDiscountCodeDataViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var discountCodeResult = await _facadeService.DiscountCodeService.CheckForDiscountPriceForPayment(model, false);

            return JsonOk(string.Empty, discountCodeResult);
        }

        public async Task<IActionResult> OnGetCheckForGiftCart(GetGiftCardCodeDataViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var giftCardResult = await _facadeService.GiftCardService.CheckForGiftCardPriceForPayment(model, false);

            return JsonOk(string.Empty, giftCardResult);
        }
    }
}
