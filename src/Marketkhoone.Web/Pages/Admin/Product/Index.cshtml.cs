using EShopMarket.Common.Helpers;
using Ganss.Xss;
using MarketKhoone.Common;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Enums.Product;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Admin.Product
{
    public class IndexModel : PageBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IUnitOfWork _uow;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public IndexModel(IFacadeServices facadeServices, IUnitOfWork uow, IHtmlSanitizer htmlSanitizer)
        {
            _facadeServices = facadeServices;
            _uow = uow;
            _htmlSanitizer = htmlSanitizer;
        }

        #endregion

        [BindProperty(SupportsGet = true)]
        public ShowProductsViewModel Products { get; set; } = new();
        public void OnGet()
        {
            //آپشن داخل سلکت ما الان ولیو دارد، اما ولیوش خالیه
            Products.SearchProducts.Categories = _facadeServices.CategoryService.GetCategoriesWithNoChild()
                .Result.CreateSelectListItem(firstItemText: "همه", firstItemValue: string.Empty);
        }
        public async Task<IActionResult> OnGetGetDataTableAsync()
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            return Partial("List", await _facadeServices.ProductService.GetProducts(Products));
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

        public async Task<IActionResult> OnPostRemoveProduct(long id)
        {
            if (id < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var product = await _facadeServices.ProductService.GetProductToRemoveInManagingProducts(id);
            if (product is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var productShortLink =
                await _facadeServices.ProductShortLinkService.FindByIdAsync(product.ProductShortLinkId);

            //چون بالا بررسی کردیم که آیا پروداکت وجود داره یا نداره
            //اینجا مطمئنیم حتما پروداکت شورت لینک وجود داره
            productShortLink.IsUsed = false;

            _facadeServices.ProductService.Remove(product);
            await _uow.SaveChangesAsync();
            foreach (var media in product.ProductMedia)
            {
                _facadeServices.UploadedFileService.DeleteFile(media.FileName,
                    media.IsVideo ? "videos" : "images", "products");
            }
            return Json(new JsonResultOperation(true, "محصول موزد نظر با موفقیت حذف شد"));
        }

        public async Task<IActionResult> OnPostRejectProduct(ProductDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, "لطفا دلایل رد برند را وارد نمایید"));
            }

            var product = await _facadeServices.ProductService.FindByIdAsync(model.Id);
            if (product is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            product.Status = ProductStatus.Rejected;
            product.RejectReason = _htmlSanitizer.Sanitize(model.RejectReason);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "محصول مورد نظر با موفقیت رد شد"));

        }

        public async Task<IActionResult> OnPostConfirmProduct(long id, Dimension dimension)
        {
            if (id < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var product = await _facadeServices.ProductService.FindByIdAsync(id);
            if (product is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            product.Status = ProductStatus.Confirmed;
            product.Dimension = dimension;
            product.RejectReason = null;
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "محصول مورد نظر با موفقیت تایید شد"));
        }

        public async Task<IActionResult> OnGetAutocompleteSearchForPersianTitle(string term)
        {
            return Json(await _facadeServices.ProductService.GetPersianTitlesForAutocomplete(term));
        }

        public async Task<IActionResult> OnGetAutocompleteSearchForShopName(string term)
        {
            return Json(await _facadeServices.SellerService.GetShopNamesForAutocomplete(term));
        }
    }
}
