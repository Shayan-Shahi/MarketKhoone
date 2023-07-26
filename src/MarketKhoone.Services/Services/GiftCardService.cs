using AutoMapper;
using DNTPersianUtils.Core;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.GiftCards;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class GiftCardService : CustomGenericService<GiftCard>, IGiftCardService
    {
        #region Constructor

        private readonly DbSet<GiftCard> _giftCards;
        private readonly DbSet<Cart> _carts;
        private readonly DbSet<Brand> _brands;
        private readonly DbSet<Category> _categories;
        private readonly IMapper _mapper;
        public GiftCardService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _giftCards = uow.Set<GiftCard>();
            _carts = uow.Set<Cart>();
            _brands = uow.Set<Brand>();
            _categories = uow.Set<Category>();
            _mapper = mapper;
        }
        #endregion

        public async Task<CheckGiftCardCodeForPaymentViewModel> CheckForGiftCardPriceForPayment(GetGiftCardCodeDataViewModel model, bool showGiftCardId)
        {
            var giftCard = await _giftCards
                .Include(x => x.Order).SingleOrDefaultAsync(x => x.Code == model.GiftCardCode);


            if (giftCard is null)
            {
                return new(false, default, "کارت هدیه پیدا نشد");
            }

            if (giftCard.Order != null)
            {
                return new(false, default, "کارت هدیه قبلا استفاده شده است");
            }

            if (giftCard.EndDateTime != null)
            {
                if (!await _carts.Where(x => x.ProductVariant.Product.Brand.Id == giftCard.BrandId.Value).AnyAsync())
                {
                    var brandTitle = await _brands.Where(x => x.Id == giftCard.BrandId.Value).Select(x => x.TitleFa).SingleOrDefaultAsync();
                    return new(false, default,
                        $"در داخل سبد خرید حداقل باید یک محثول از برند{brandTitle} وجود داشته باشد");
                }
            }

            if (giftCard.CategoryId != null)
            {
                if (!await _carts.Where(x => x.ProductVariant.Product.Category.Id == giftCard.CategoryId.Value).AnyAsync())
                {
                    var categoryTitle = await _categories.Where(x => x.Id == giftCard.CategoryId.Value).Select(x => x.Title).SingleOrDefaultAsync();
                    return new(false, default,
                        $"در داخل سبد خرید باید حداقل یک محصول از دسته بندی{categoryTitle} وجود داشته باشد");
                }
            }

            if (giftCard.MinimumPriceOfCart > model.SumPriceOfCart)
            {
                return new(false, default,
                    $"مجموع قیمت محصولات داخل سبد خرید باید مساوی و یا بیشتر از {giftCard.MinimumPriceOfCart.Value.ToString("#, 0").ToPersianNumbers()}باشد ");
            }

            if (showGiftCardId)
            {
                return new(true, giftCard.Price, null, giftCard.Id);
            }

            return new(true, giftCard.Price);
        }

        //public Task<(bool Result, string Message)> CheckForGiftCardCodeInVerify(Order order)
        //{

        //}
    }
}
