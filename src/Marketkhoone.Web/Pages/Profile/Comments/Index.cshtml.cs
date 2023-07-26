using MarketKhoone.Common.Constants;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.ProductComments;
using MarketKhoone.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketkhoone.Web.Pages.Profile.Comments
{
    public class IndexModel : PageModel
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        public IndexModel(IFacadeServices facadeServices)
        {
            _facadeServices = facadeServices;
        }

        #endregion

        public ShowProductsInProfileCommentViewModel Products { get; set; } = new();

        public ShowProductCommentsInProfile Comments { get; set; } = new();

        public bool IsActiveTabPending { get; set; }
        public async Task<IActionResult> OnGet(string activeTab = "pending")
        {
            if (activeTab != "pending" && activeTab != "comments")
            {
                return RedirectToPage(PublicConstantStrings.Error500PageName);
            }

            IsActiveTabPending = activeTab == "pending";
            if (IsActiveTabPending)
            {
                Products = await _facadeServices.ParcelPostItemService.GetProductsInProfileComment(Products);
            }
            else
            {
                Comments = await _facadeServices.ProductCommentService.GetCommentsInProfileComment(Comments);
            }

            return Page();
        }
        /// <summary>
        /// نمایش نظرات به صورت صفحه بندی شده
        ///
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetShowCommentsByPagination(int pageNumber)
        {
            var comments
                = await _facadeServices.ProductCommentService.GetCommentsInProfileComment(pageNumber);
            return Partial("_Comments", comments);
        }
        /// <summary>
        /// نمایش نظرات به صورت صفحه بندی شده
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<IActionResult> ONGetShowProductsByPagination(int pageNumber)
        {
            var products = await _facadeServices.ParcelPostItemService.GetProductsInProfileComment(pageNumber);
            return Partial("_Products", products);
        }
    }
}
