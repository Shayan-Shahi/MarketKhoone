using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.ProductShortLinks;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Admin.ProductShortLink
{
    public class IndexModel : PageBase
    {
        #region Constructor
        private readonly IFacadeServices _facadeServices;
        public readonly IUnitOfWork _uow;

        public IndexModel(IFacadeServices facadeServices, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _uow = uow;
        }
        #endregion

        [BindProperty(SupportsGet = true)]
        public ShowProductShortLinksViewModel ProductShortLinks { get; set; } = new();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnGetGetDataTableAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, PublicConstantStrings.ModelStateErrorMessage);
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            return Partial("List", await _facadeServices.ProductShortLinkService.GetProductShortLinks(ProductShortLinks));
        }

        public async Task<IActionResult> OnPostDelete(long shortLinkId)
        {
            if (shortLinkId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var shortLink = await _facadeServices.ProductShortLinkService.GetForDelete(shortLinkId);

            if (shortLink is null)
            {
                return Json(new JsonResultOperation(false));
            }
            _facadeServices.ProductShortLinkService.Remove(shortLink);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "لینک مورد نظر با موفقیت حذف شد"));
        }
    }
}
