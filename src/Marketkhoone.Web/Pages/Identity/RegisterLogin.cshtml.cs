using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Identity;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Identity;
using MarketKhoone.ViewModels.Identity.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Marketkhoone.Web.Pages.Identity
{
    public class RegisterLoginModel : PageModel
    {
        private readonly IApplicationUserManager _userManager;
        private readonly ILogger<RegisterLoginModel> _logger;
        private readonly SiteSettings _siteSettings;
        private readonly IUnitOfWork _uow;
        private readonly IFacadeServices _facadeServices;
        public RegisterLoginModel(IApplicationUserManager userManager, ILogger<RegisterLoginModel> logger,
            IOptionsMonitor<SiteSettings> siteSettings, IUnitOfWork uow, IFacadeServices facadeServices)
        {
            _userManager = userManager;
            _logger = logger;
            _uow = uow;
            _siteSettings = siteSettings.CurrentValue;
            _facadeServices = facadeServices;
        }
        [BindProperty]
        public RegisterLoginViewModel RegisterLogin { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(RegisterLoginViewModel registerLogin)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, PublicConstantStrings.ModelStateErrorMessage);
                return Page();
            }

            var isInputEmail = registerLogin.PhoneNumberOrEmail.IsEmail();
            if (!isInputEmail)
            {
                var addNewUser = false;
                var user = await _userManager.FindByNameAsync(registerLogin.PhoneNumberOrEmail);
                if (user is null)
                {

                    user = new User
                    {
                        UserName = registerLogin.PhoneNumberOrEmail,
                        PhoneNumber = registerLogin.PhoneNumberOrEmail,
                        Avatar = _siteSettings.UserDefaultAvatar,
                        Email = $"{StringHelpers.GenerateGuid()}@test.com",
                        SendSmsLastTime = DateTime.Now
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        await _uow.SaveChangesAsync();
                        _logger.LogInformation(LogCodes.RegisterCode,
                            $"{user.UserName} created a new account with phone number");
                        addNewUser = true;
                    }
                    else
                    {
                        ModelState.AddErrorsFromResult(result);
                        return Page();
                    }
                }
                if (DateTime.Now > user.SendSmsLastTime.AddMinutes(3) || addNewUser)
                {
                    var phoneNumberToken =
                        await _userManager.GenerateChangePhoneNumberTokenAsync(user, RegisterLogin.PhoneNumberOrEmail);

                    //todo: Commented for development, UNCOMMENT when publishing
                    //  _facadeServices.SendSmsService.LoginCodeForUserInWebsiteOne(registerLogin.PhoneNumberOrEmail, phoneNumberToken);
                    user.SendSmsLastTime = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    await _uow.SaveChangesAsync();
                }
            }
            return RedirectToPage("./LoginWithPhoneNumber", new { phoneNumber = registerLogin.PhoneNumberOrEmail });
        }
    }
}
