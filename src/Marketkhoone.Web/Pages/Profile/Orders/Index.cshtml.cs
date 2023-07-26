using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketkhoone.Web.Pages.Profile.Orders
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

        public ShowOrdersInProfileViewModel Orders { get; set; } = new();
        public async Task OnGet()
        {
            Orders = await _facadeServices.OrderService.GetOrdersInProfile(Orders);
        }


        public async Task<IActionResult> OnGetShowOrderByPagination(int pageNumber)
        {
            var orders = await _facadeServices.OrderService.GetOrdersInProfile(pageNumber);
            return Partial("_Orders", orders);
        }
    }
}
