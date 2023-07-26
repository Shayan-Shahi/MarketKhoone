using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Marketkhoone.Web.Pages.Identity
{
    public class LoginWithPhoneNumberModel : PageBase
    {
        #region Constructor
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IFacadeServices _facadeServices;
        private readonly IUnitOfWork _uow;

        public LoginWithPhoneNumberModel(IApplicationUserManager userManager,
            IApplicationSignInManager signInManager, IFacadeServices facadeServices, IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _facadeServices = facadeServices;
            _uow = uow;
        }
        #endregion

        public LoginWithPhoneNumberViewModel LoginWithPhoneNumber { get; set; } = new();

        [ViewData]
        public string ActivationCode { get; set; }

        public async Task<IActionResult> OnGetAsync(string phoneNumber)
        {
            var userSendSmsLastTime = await _userManager.GetSendSmsLastTimeAsync(phoneNumber);
            if (userSendSmsLastTime is null)
            {
                return RedirectToPage();
            }
            #region Development
            var user = await _userManager.FindByNameAsync(phoneNumber);
            var phoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            ActivationCode = phoneNumberToken;
            #endregion

            //todo: Uncomment when publishing
            // _facadeServices.SendSmsService.ProfileIsCreatedMessageToUserInWebsiteTwo(phoneNumber);
            var (min, sec) = userSendSmsLastTime.Value.GetMinuteAndSecondForLoginWithPhoneNumberPage();
            LoginWithPhoneNumber.SendSmsLastTimeMinute = min;
            LoginWithPhoneNumber.SendSmsLastTimeSecond = sec;
            LoginWithPhoneNumber.PhoneNumber = phoneNumber;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(LoginWithPhoneNumberViewModel LoginWithPhoneNumber)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, "مقادیر را به درستی وارد نمایید"));
            }

            var user = await _userManager.FindByNameAsync(LoginWithPhoneNumber.PhoneNumber);
            if (user is null)
            {
                return Json(new JsonResultOperation(false));
            }

            var result = await _userManager.VerifyChangePhoneNumberTokenAsync(user, LoginWithPhoneNumber.ActivationCode,
                LoginWithPhoneNumber.PhoneNumber);
            if (!result)
            {
                return Json(new JsonResultOperation(false, "کد وارد شده صحیح نمی باشد"));

            }

            user.IsActive = true;
            await _signInManager.SignInAsync(user, true);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, " با موفقیت وارد شدید"));

        }


        public async Task<IActionResult> OnPostReSendUserSmsActivationAsync(string phoneNumber)
        {
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user is null)
                return Json(new JsonResultOperation(false));

            if (user.SendSmsLastTime.AddMinutes(3) > DateAndTime.Now)
                return Json(new JsonResultOperation(false));

            var phoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            // Sending the LoginCode Again(by the request of user)
            //Todo: Commented for development, UNCOMMENT when publishing.
            //_facadeServices.SendSmsService.LoginCodeForUserInWebsiteOne(phoneNumber, phoneNumberToken);

            user.SendSmsLastTime = DateAndTime.Now;
            return Json(new JsonResultOperation(true, "کد ورود مجددا ارسال گردید")
            {
                Data = new
                {
                    activationCode = phoneNumberToken
                }
            });
        }
    }
}
