using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.UserHistories;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Profile.UserHistory
{
    public class IndexModel : ProfilePageBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IUnitOfWork _uow;

        public IndexModel(IFacadeServices facadeServices, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _uow = uow;
        }

        #endregion

        public List<ShowUserHistoryViewModel> Products { get; set; }
        public async Task OnGet()
        {
            Products = await _facadeServices.UserHistoryService.GetUserHistories();
        }

        public async Task<IActionResult> OnPost(long productId)
        {
            var userId = User.Identity.GetLoggedInUserId();

            var userHistory = await _facadeServices.UserHistoryService.FindAsync(userId, productId);

            if (userHistory != null)
            {
                _facadeServices.UserHistoryService.Remove(userHistory);
                await _uow.SaveChangesAsync();
            }

            return JsonOk("محصول مورد نظر با موفقیت از بازدید های اخیر شما حذف شد", new
            {
                ProductId = productId
            });
        }
    }
}
