using EShopMarket.Common.Helpers;
using MarketKhoone.Common;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.SellerPanel.Product
{
    public class AllModel : SellerPanelBase
    {

        #region Constructor
        private readonly IFacadeServices _facadeService;
        public AllModel(IFacadeServices facadeService)
        {
            _facadeService = facadeService;
        }
        #endregion

        [BindProperty(SupportsGet = true)]
        public ShowAllProductsInSellerPanelViewModel Products { get; set; } = new(); 
        public void OnGet()
        {
            Products.SearchProducts.Categories = _facadeService.CategoryService.GetCategoriesWithNoChild()
                .Result.CreateSelectListItem(firstItemText: "aaa", firstItemValue: string.Empty);
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

            return Partial("ListForAll", await _facadeService.ProductService.GetAllProductsInSellerPanel(Products));
        }

        public async Task<IActionResult> OnGetGetProductDetails(long productId)
        {
            var product = await _facadeService.ProductService.GetProductDetails(productId);
            if (product is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("ProductDetails", product);
        }

        public async Task<IActionResult> OnGetAutocompleteSearchPersianTitle(string term)
        {
            return Json(await _facadeService.ProductService.GetPersianTitlesForAutocomplete(term));
        }
    }


}
