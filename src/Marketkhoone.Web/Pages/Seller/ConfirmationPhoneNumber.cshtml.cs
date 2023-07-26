using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Sellers;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Seller
{
    public class ConfirmationPhoneNumberModel : PageBase
    {
        #region Constructor

        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IUnitOfWork _uow;
        private readonly IFacadeServices _facadeServices;

        public ConfirmationPhoneNumberModel(IApplicationUserManager userManager,
            IUnitOfWork uow, IFacadeServices facadeServices, IApplicationSignInManager signInManager)
        {
            _userManager = userManager;
            _uow = uow;
            _facadeServices = facadeServices;
            _signInManager = signInManager;
        }

        #endregion


        [TempData]
        public string ActivationCode { get; set; }
        [BindProperty(SupportsGet = true)]
        public ConfirmationSellerPhoneNumberViewModel Confirmation { get; set; }
            = new();

        public async Task<IActionResult> OnGetAsync(string phoneNumber)
        {
            var userSendSmsLastTime = await _userManager.GetSendSmsLastTimeAsync(phoneNumber);
            if (userSendSmsLastTime is null)
            {
                return RedirectToPage("/Error");
            }

            #region Development

            var user = await _userManager.FindByNameAsync(phoneNumber);
            var phoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            ActivationCode = phoneNumberToken;

            #endregion

            var (min, sec) = userSendSmsLastTime.Value.GetMinuteAndSecondForLoginWithPhoneNumberPage();
            Confirmation.SendSmsLastTimeMinute = min;
            Confirmation.SendSmsLastTimeSecond = sec;
            Confirmation.PhoneNumber = phoneNumber;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, "مقادیر را به درستی وارد نمایید")
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            var user = await _userManager.FindByNameAsync(Confirmation.PhoneNumber);
            if (user is null)
            {
                return Json(new JsonResultOperation(false, "شماره تلفن مورد نظر یافت نشد"));
            }

            var result = await _userManager.VerifyChangePhoneNumberTokenAsync(user, Confirmation.ActivationCode, Confirmation.PhoneNumber);
            if (!result)
            {
                return Json(new JsonResultOperation(false, "کد وارد شده صحیح نمیباشد"));
            }

            user.IsSeller = true;
            await _userManager.UpdateAsync(user);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "شماره تلفن شما با موفقیت تایید شد")
            {
                Data = Confirmation.PhoneNumber
            });
        }

        public async Task<IActionResult> OnPostReSendSellerSmsActivationAsync(string phoneNumber)
        {

            //System.Threading.Thread.Sleep(2000);
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user is null)
                return Json(new JsonResultOperation(false));
            if (user.SendSmsLastTime.AddMinutes(3) > DateTime.Now)
                return Json(new JsonResultOperation(false));
            var phoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            // todo: Uncomment when publishing

            _facadeServices.SendSmsService.LoginCodeForUserInWebsiteOne(phoneNumber, phoneNumberToken);

            user.SendSmsLastTime = DateTime.Now;
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "کد فعال سازی مجددا ارسال شد")
            {
                Data = new
                {
                    activationCode = phoneNumberToken
                }
            });
        }

    }
}
