using DNTPersianUtils.Core;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.ViewModels.DiscountCodes;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class DiscountCodeService : GenericService<DiscountCode>, IDiscountCodeService
    {
        #region Constructor

        private readonly DbSet<DiscountCode> _discountCodes;
        private readonly DbSet<Cart> _carts;
        private readonly DbSet<Brand> _brands;
        private readonly DbSet<Category> _categories;
        private readonly DbSet<UsedDiscountCode> _usedDiscountCode;
        private readonly DbSet<ParcelPostItem> _parcelPostItems;
        private readonly IApplicationUserManager _userManger;

        public DiscountCodeService(IUnitOfWork uow, IApplicationUserManager userManger) : base(uow)
        {
            _userManger = userManger;
            _discountCodes = uow.Set<DiscountCode>();
            _carts = uow.Set<Cart>();
            _brands = uow.Set<Brand>();
            _categories = uow.Set<Category>();
            _usedDiscountCode = uow.Set<UsedDiscountCode>();
            _parcelPostItems = uow.Set<ParcelPostItem>();
        }
        #endregion

        public async Task<CheckDiscountCodeForPaymentViewModel> CheckForDiscountPriceForPayment(GetDiscountCodeDataViewModel model, bool showDiscountCodeId)
        {
            var discountCode = await _discountCodes.SingleOrDefaultAsync(x => x.Code == model.DiscountCode);

            var userId = _userManger.GetLoggedInUser();
            if (discountCode is null)
            {
                return new(false, default, "کد تخفیف پیدا نشد");
            }

            var now = DateTime.Now;

            if (discountCode.StartDateTime > now)
            {
                return new(false, default, "زمان کد تخفیف شروع نشده است");
            }

            if (now > discountCode.EndDateTime)
            {
                return new(false, default, "کد تخفیف منقضی شده است");
            }

            if (discountCode.BrandId != null)
            {
                if (!await _carts.Where(x => x.ProductVariant.Product.Brand.Id == discountCode.BrandId.Value).AnyAsync(x => x.UserId == userId))
                {
                    var brandTitle = await _brands.Where(x => x.Id == discountCode.BrandId.Value).Select(x => x.TitleFa)
                        .SingleOrDefaultAsync();

                    return new(false, default,
                        $"در داخل سبد خرید حداقل یه محصول باید از  برند {brandTitle}وجود داشته باشد");
                }
            }

            if (discountCode.CategoryId != null)
            {
                if (!await _carts.Where(x => x.ProductVariant.Product.Category.Id == discountCode.CategoryId.Value).AnyAsync())
                {
                    var categoryTitle = await _categories.Where(x => x.Id == discountCode.CategoryId.Value)
                        .Select(x => x.Title).SingleOrDefaultAsync();
                    return new(false, default,
                        $"در داخل سبد خرید باید حداقل یک محصول از دسته بندی{categoryTitle}وجود داشته باشد ");
                }
            }

            if (discountCode.MinimumPriceOfCart > model.SumPriceOfCart)
            {
                return new(false, default,
                    $"مجموع قیمت محصولات داخل سبد خرید باید مساوی و یا بیشتر از {discountCode.MinimumPriceOfCart.Value.ToString("#,0").ToString().ToPersianNumbers()} باشد");
            }

            if (discountCode.LimitedCount != null)
            {
                var usedDiscountCodesCountByCuerentUser =
                    await _usedDiscountCode.Where(x => x.DiscountCodeId == discountCode.Id).CountAsync();


                //چرا از بزرگتر مساوی استفاده میکنیم
                // لیمیت کانت کاربر 5 تاست
                //سه بار از کد تخفیف استفاده میکنه
                //یعد لیمیت رو  ویرایش میکنیم به 2
                //حالا شرط پایین برقرار میشه و چون استفاده شده (3) بزگتر از لیمیت کاربر هست (2) در نتیجه
                // متن خظا رو نمایش میده
                // در صورتیکه اگر از مساوی برای شذط پایین استفاده میکیردیم، اصلا خظا رو نمایش نمیداد

                if (usedDiscountCodesCountByCuerentUser >= discountCode.LimitedCountByOneUser)
                {
                    return new(false, default,
                        $"ظرفیت استفاده از این کد تخفیف برای هر کاربر {discountCode.LimitedCountByOneUser}بار است و شما حداکثر استفاده از این کد تخفیف را داشته اید");
                }
            }

            if (showDiscountCodeId)
            {
                return new CheckDiscountCodeForPaymentViewModel(true, discountCode.Price, null, discountCode.Id);
            }
            return new(true, discountCode.Price);
        }

        public async Task<(bool Resullt, string Message)> CheckForDiscountCodeInVerify(Order order)
        {
            var discountCode = await _discountCodes.SingleAsync(x => x.Id == order.DiscountCodeId);

            var now = DateTime.Now;

            if (discountCode.LimitedCount != null)
            {
                var usedDiscountCodeCount = await _usedDiscountCode
                    .Where(x => x.DiscountCodeId == discountCode.Id).CountAsync();

                if (usedDiscountCodeCount >= discountCode.LimitedCount)
                {
                    return (false,
                        $"متاسفانه ظرفیت استفاده از کد تخفیف {discountCode.Code}در زمانیکه پرداخت خود را انجام میدادید به پایان رسید");

                }
            }

            if (discountCode.StartDateTime > now)
            {
                return (false,
                    $"متاسفانه زمان شروع کد تخفیف در زمانیکه پرداخت خود را انجام میدادید تغییر پیدا کرده است و در حال حاضز امکان استفاده از کد تخفیف {discountCode.Code}وجود ندارد ");
            }

            if (now > discountCode.EndDateTime)
            {
                return (false,
                    $"متاسفانه مهلت زمانی کد تخفیف {discountCode.Code} در زمانیکه پرداخت خود را انجام میدادید به میزان {discountCode.Price.ToString("#, 0").ToPersianNumbers()}تومان تغییر پیدا کرده است");

            }

            if (discountCode.MinimumPriceOfCart != null)
            {
                if (discountCode.MinimumPriceOfCart >
                    order.FinalPrice + order.DiscountCodePrice + order.GiftCardCodePrice)
                {
                    return (false,
                        $"متاسفانه حداقل مبلغ سبد خرید برای استفاده از کد تخفیف{discountCode.Code} به مبلغ {discountCode.MinimumPriceOfCart.Value.ToString("#,0").ToPersianNumbers()}تومان تغییر پیدا کرده است");
                }
            }

            if (discountCode.BrandId != null)
            {
                if (!await _parcelPostItems.Where(x => x.Order.Id == order.Id).Where(x =>
                        x.ProductVariant.Product.Category.Id == discountCode.CategoryId.Value).AnyAsync())
                {
                    var categoryTitle = await _categories.Where(x => x.Id == discountCode.CategoryId.Value).Select(x => x.Title).SingleOrDefaultAsync();
                    return (false,
                        $"متاسفانه کد تخفیف {discountCode.Code}در زمانیکه پرداخت خود را انجام می دادی، دسته بندی آن تغییر پیدا رده است، در حال حاضر برای ایتفاده از این کد تخفیف باید یک محثول از دسته بندی {categoryTitle}بایددر داخل سفارش شما وجود داشته باشد ");

                }
            }

            return (true, default);
        }
    }
}
