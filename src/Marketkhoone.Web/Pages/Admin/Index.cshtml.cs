using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketkhoone.Web.Pages.Admin
{
    public class IndexModel : PageModel
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IApplicationUserManager _userManager;

        public IndexModel(IFacadeServices facadeServices, IApplicationUserManager userManager)
        {
            _facadeServices = facadeServices;
            _userManager = userManager;
        }
        #endregion

        public AdminMainPageViewModel AdminMainPage { get; set; } = new();
        public async Task OnGet()
        {
            AdminMainPage = new AdminMainPageViewModel()
            {
                ProductTotalCounts = await _facadeServices.ProductService.GetTotalProductCounts(),
                ProductVariantIsSellingNow = await _facadeServices.ProductVariantService.GetProductsSellingNow(),
                OrdersNotConfirmedCounts = await _facadeServices.OrderService.GetAllNewOrdersCount(),
                UsersTotalCounts = await _userManager.GetAllUsersCount(),
                CommentsNotConfirmedCounts = await _facadeServices.ProductCommentService.GetAllNewProductComments(),
                ReturnProductsCounts = 0

            };
        }
    }
}
