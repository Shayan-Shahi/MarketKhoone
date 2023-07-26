using MarketKhoone.Common.Helpers;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.ViewModels.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Marketkhoone.Web.Pages.Identity
{
    public class LoginWithPasswordModel : PageModel
    {
        #region Constructor
        private readonly IApplicationUserManager _userManager;
        public LoginWithPasswordModel(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }
        #endregion

        public LoginWithPhoneNumberViewModel LoginWithPhoneNumber { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(string phoneNumber)
        {
            var userSendSmsLastTime = await _userManager.GetSendSmsLastTimeAsync(phoneNumber);

            if (userSendSmsLastTime is null)
            {
                return RedirectToPage("/Error");
            }

            var (min, sec) = userSendSmsLastTime.Value.GetMinuteAndSecondForLoginWithPhoneNumberPage();
            LoginWithPhoneNumber.SendSmsLastTimeMinute = min;
            LoginWithPhoneNumber.SendSmsLastTimeSecond = sec;
            LoginWithPhoneNumber.PhoneNumber = phoneNumber;
            return Page();
        }
    }
}
