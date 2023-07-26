using AutoMapper;
using DNTPersianUtils.Core;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common;
using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Products;
using MarketKhoone.ViewModels.ProductVariants;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.SellerPanel.Product
{
    [CheckModelStateInRazorPages]
    public class IndexModel : SellerPanelBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public IndexModel(IFacadeServices facadeServices, IMapper mapper, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _mapper = mapper;
            _uow = uow;
        }

        #endregion

        [BindProperty(SupportsGet = true)]
        public ShowProductsInSellerPanelViewModel Products { get; set; } = new();
        public void OnGet()
        {
            Products.SearchProducts.Categories = _facadeServices.CategoryService.GetSellerCategories()
                .Result.CreateSelectListItem(firstItemText: "همه", firstItemValue: string.Empty);
        }

        public async Task<IActionResult> OnGetGetDataTableAsync()
        {
            return Partial("List", await _facadeServices.ProductService.GetProductsInSellerPanel(Products));
        }

        public async Task<IActionResult> OnGetGetProductDetails(long productId)
        {
            var product = await _facadeServices.ProductService.GetProductDetails(productId);
            if (product is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("ProductDetails", product);
        }

        public async Task<IActionResult> OnGetAutocompleteSearchForPersianTitle(string term)
        {
            return Json(await _facadeServices.ProductService.GetPersianTitlesForAutocompleteInSellerPanel(term));
        }

        public async Task<IActionResult> OnGetShowProductVariantsAsync(long productId)
        {
            if (productId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            return Partial("ProductVariants",
                await _facadeServices.ProductVariantService.GetProductVariants(productId));
        }

        public async Task<IActionResult> OnGetEditProductVariant(long productVariantId)
        {
            if (productVariantId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var productVariant = await _facadeServices.ProductVariantService.GetDataForEdit(productVariantId);

            if (productVariant is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("_EditProductVariantPartial", productVariant);
        }

        public async Task<IActionResult> OnPostEditProductVariant(EditProductVariantViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage));
            }

            var productVariantToEdit = await _facadeServices.ProductVariantService.GetForEdit(model.Id);

            if (productVariantToEdit is null)
            {
                return Json(new JsonResultOperation(false));
            }

            productVariantToEdit = _mapper.Map(model, productVariantToEdit);
            productVariantToEdit.StartDateTime = productVariantToEdit.EndDateTime = null;
            productVariantToEdit.OffPrice = productVariantToEdit.OffPercentage = null;

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "تنوع محصول نظر با موفقیت ویرایش شد"));
        }

        public async Task<IActionResult> OnGetAddEditDiscount(long productVariantId)
        {
            if (productVariantId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var productVariant =
                await _facadeServices.ProductVariantService.GetDataForAddEditDiscount(productVariantId);

            if (productVariant is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("_AddEditDiscountPartial", productVariant);
        }

        public async Task<IActionResult> OnPostAddEditDiscount(AddEditDiscountViewModel model)
        {
            var productVariant = await _facadeServices.ProductVariantService.GetForEdit(model.Id);
            if (productVariant is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var parsedDateTimes =
                DateTimeHelper.ConvertDateTimeForAddEditDiscount(model.StartDateTime, model.EndDateTime);
            if (!parsedDateTimes.IsSuccessful)
            {
                return Json(new JsonResultOperation(false, "لطفا تاریخ ها را به درستی وارد نمایید"));
            }

            if (parsedDateTimes.IsStartDateTimeGreatherOrEqualEndDateTime)
            {
                return Json(new JsonResultOperation(false, "تاریخ پایان تخفیف باید بزرگتر از تاریخ شروع تخفیف باشد"));

            }

            if (parsedDateTimes.IsTimeSpanLowerThan3Hour)
            {
                return Json(new JsonResultOperation(false,
                    "تاریخ پایان تخفیف باید حداقل 3 ساعت بزگتر از تاریخ شروع تخفیف باشد"));
            }

            var offPrice = model.OffPrice;
            var price = productVariant.Price;
            var offPercentage = model.OffPercentage;
            var discountPrice = price / 100 * offPercentage;
            var priceWithDiscount = price - discountPrice;
            var discountPricesSubtract1Percentage = price / 100 * (offPercentage - 1);
            var priceWithDiscountSubtract1Percentage = price - discountPricesSubtract1Percentage;


            if (offPrice < priceWithDiscount || offPrice >= priceWithDiscountSubtract1Percentage)
            {
                return Json(new JsonResultOperation(false,
                    $"قیمت تخفیف باید بزگتر و مساوی{priceWithDiscount.ToString("#,0").ToPersianNumbers()}تومان و کوچکتر از {priceWithDiscountSubtract1Percentage.ToString("#,0").ToPersianNumbers()}تومان باشد"));
            }

            productVariant.StartDateTime = parsedDateTimes.StartDate;
            productVariant.EndDateTime = parsedDateTimes.EndDate;
            _mapper.Map(model, productVariant);

            // todo: Send notices to the users
            // todo: remove all notices records

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "تنوع محصول مورد نظر با موفقیت ویرایش شد"));
        }
    }
}
