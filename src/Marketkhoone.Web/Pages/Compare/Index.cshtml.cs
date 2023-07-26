using DNTCommon.Web.Core;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Compare
{
    public class IndexModel : PageBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IViewRendererService _viewRendererService;

        public IndexModel(IFacadeServices facadeServices, IViewRendererService viewRendererService)
        {
            _facadeServices = facadeServices;
            _viewRendererService = viewRendererService;
        }

        #endregion

        public List<ShowProductInCompareViewModel> Products { get; set; }
        public async Task<IActionResult> OnGet(int productCode1, int productCode2, int productCode3, int productCode4)
        {
            if (productCode1 < 1)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            if (!await _facadeServices.CategoryService
                    .CheckProductCategoryIdsInComparePage(productCode1, productCode2, productCode3, productCode4))
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            Products = await _facadeServices.ProductService
                .GetProductForCompare(productCode1, productCode2, productCode3, productCode4);

            return Page();
        }

        public async Task<IActionResult> OnGetGetProductsForCompare(int productCode1, int productCode2, int productCode3,
            int productCode4)
        {
            if (productCode1 < 1)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            if (!await _facadeServices.CategoryService
                    .CheckProductCategoryIdsInComparePage(productCode1, productCode2, productCode3, productCode4))
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            Products = await _facadeServices.ProductService
                .GetProductForCompare(productCode1, productCode2, productCode3, productCode4);

            return Partial("_CompareBodyPartial", Products);
        }


        public async Task<IActionResult> OnGetShowAddProduct(int[] productCodesToHide, int pageNumber = 1,
            string searchValue = "")
        {
            if (!productCodesToHide.Any())
            {
                return Json(new JsonResultOperation(false));
            }

            var result =
                await _facadeServices.ProductService.GetProductsForAddProductInCompare(pageNumber, searchValue,
                    productCodesToHide);

            //چرا برای این ایف از پارامتر ورودی هندلر استفاده نمیکنیم؟
            //چون امکان داره که عدد صفر رو وارد کنه
            // . اگر صفر وارد بشه، وارد الس میشه و پارشل صفحه یک به یالا را نمایش میده
            //به خاطر همین از پیچ نامبری استفاده کردیم که مطمئنیم عدد درست رو داره
            // یعنی پیج نامبری که خودمون داخل سروریس مقدار دهی میکنیم
            if (result.PageNumber == 1)
            {
                //صفحه یک
                // کل محتویات داخل مودال را تغییر میدهیم
                return Json(new JsonResultOperation(true, string.Empty)
                {
                    //به تعداد محصولات نیاز نداریم
                    // چون در داخل خود پارشل، تعداد محصولات نمایش داده نمی شوند
                    Data = new
                    {
                        ProductsBody = await _viewRendererService
                            .RenderViewToStringAsync("~/Pages/Compare/_AddProductPartial.cshtml", result),
                        PageNumber = result.PageNumber,
                        IsLastPage = result.IsLastPage
                    }
                });
            }
            else
            {
                return Json(new JsonResultOperation(true, string.Empty)
                {
                    Data = new
                    {
                        ProductsBody = await _viewRendererService
                            .RenderViewToStringAsync("~/Pages/Compare/_ProductsInAddProductPartial.cshtml",
                                result.Products),
                        ProductsCount = result.Count,
                        PageNumber = result.PageNumber,
                        IsLastPage = result.IsLastPage
                    }
                });
            }
        }
    }
}
