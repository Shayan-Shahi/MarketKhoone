using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketkhoone.Web.Pages.Search
{
    public class ShowCategoryModel : PageModel
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        public ShowCategoryModel(IFacadeServices facadeServices)
        {
            _facadeServices = facadeServices;
        }

        #endregion

        public SearchOnCategoryViewModel SearchOnCategory { get; set; }
        public async Task OnGet(string categorySlug, string brandSlug)
        {
            SearchOnCategory = await _facadeServices.CategoryService.GetSearchOnCategoryData(categorySlug, brandSlug);
        }
        /// <summary>
        /// نمایش محصولات به صورت صفحه بندی شده
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetShowProductsByPagination(SearchOnCategoryInputsViewModel inputs)
        {
            var result = await _facadeServices.ProductService.GetProductsByPaginationForSearch(inputs);
            return Partial("_Products", result);
        }
    }
}
