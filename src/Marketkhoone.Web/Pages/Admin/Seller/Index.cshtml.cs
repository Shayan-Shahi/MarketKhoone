using EShopMarket.Common.Helpers;
using Ganss.Xss;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Enums.Seller;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Sellers;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Admin.Seller
{
    public class IndexModel : PageBase
    {
        #region Costructor

        private readonly IFacadeServices _facadeServices;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IUnitOfWork _uow;

        public IndexModel(IFacadeServices facadeServices, IHtmlSanitizer htmlSanitizer, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _htmlSanitizer = htmlSanitizer;
            _uow = uow;
        }
        #endregion

        [BindProperty(SupportsGet = true)]
        public ShowSellersViewModel Sellers { get; set; } = new();
        public void OnGet()
        {
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
            return Partial("List", await _facadeServices.SellerService.GetSellers(Sellers));
        }


        public async Task<IActionResult> OnGetGetSellerDetails(long sellerId)
        {

            var seller = await _facadeServices.SellerService.GetSellerDetails(sellerId);

            if (seller is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }
            return Partial("SellerDetails", seller);

        }

        public async Task<IActionResult> OnPostRejectSellerDocuments(SellerDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, "لطفا دلایل رد مدارک فروشنده را وارد نمایید"));
            }

            var seller = await _facadeServices.SellerService.FindByIdAsync(model.Id);
            if (seller is null)
            {
                return Json(new JsonResultOperation(false, "فروشنده مورد نظر یافت نشد"));
            }

            seller.DocumentStatus = DocumentStatus.Rejected;
            seller.RejectReason = _htmlSanitizer.Sanitize(model.RejectReason);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "مدارک فروشنده مورد نظر با موفقیت رد شد"));
        }

        public async Task<IActionResult> OnPostConfirmSellerDocuments(long id)
        {
            if (id < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var seller = await _facadeServices.SellerService.FindByIdAsync(id);
            if (seller is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            seller.DocumentStatus = DocumentStatus.Confirmed;
            seller.RejectReason = null;
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "مدارک فروشنده مورد نظر با موفقیت تایید شد"));
        }

        public async Task<IActionResult> OnPostRemoveUser(long id)
        {
            if (id < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var seller = await _facadeServices.SellerService.GetSellerToRemoveInManagingSellers(id);
            if (seller is null)
            {
                return Json(new JsonResultOperation(false, "فروشنده مورد نظر یافت نشد"));
            }
            _facadeServices.SellerService.Remove(seller);
            
            seller.User.IsSeller = false;
            //ولی مشکل اینه ، ما در اینجا به 
            //User
            //دسترسی نداریم
            //باید در سروریس این اکشن
            //.Include(x=>x.User)
            //رو اینکلود کنیم تا به یوزر دسترسی پیدا کنیم


            //await _uow.SaveChangesAsync();
            //_facadeServices.UploadedFileService.DeleteFile(seller.IdCartPicture, "images", "seller-id-cart-pictures");
            //_facadeServices.UploadedFileService.DeleteFile(seller.Logo, "images", "seller-logos");
            return Json(new JsonResultOperation(true, "فروشنده مورد نظر با موفقیت حذف شد"));
        }
    }
}
