using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Identity;
using MarketKhoone.Services.Contracts.Identity;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Identity.Settings;
using MarketKhoone.ViewModels.Sellers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Marketkhoone.Web.Pages.Seller
{
    public class RegisterModel : PageModel
    {
        #region Constructor

        private readonly IApplicationUserManager _userManager;
        private readonly IFacadeServices _facadeServices;
        private readonly SiteSettings _siteSettings;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IUnitOfWork _uow;

        public RegisterModel(IApplicationUserManager userManager,
            IOptionsMonitor<SiteSettings> siteSettings,
            ILogger<RegisterModel> logger, IFacadeServices facadeServices, IUnitOfWork uow)
        {
            _userManager = userManager;
            _logger = logger;
            _facadeServices = facadeServices;
            _uow = uow;
            _siteSettings = siteSettings.CurrentValue;
        }

        #endregion

        [BindProperty]
        public RegisterSellerViewModel RegisterSeller { get; set; } = new();
        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, PublicConstantStrings.ModelStateErrorMessage);
                return Page();
            }

            var addNewUser = false;
            var user = await _userManager.FindByNameAsync(RegisterSeller.PhoneNumber);
            if (user is null)
            {
                user = new User
                {
                    UserName = RegisterSeller.PhoneNumber,
                    PhoneNumber = RegisterSeller.PhoneNumber,
                    Avatar = _siteSettings.UserDefaultAvatar,
                    Email = RegisterSeller.Email,

                };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await _uow.SaveChangesAsync();
                    _logger.LogInformation(LogCodes.RegisterCode, $"{user.UserName} created a new account with phone number");
                    addNewUser = true;
                }
                else
                {
                    ModelState.AddErrorsFromResult(result);
                    return Page();
                }
            }
            if (DateTime.Now > user.SendSmsLastTime.AddMinutes(1) || addNewUser)
            {
                var phoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, RegisterSeller.PhoneNumber);

                // todo: Uncomment when publishing
                // _facadeServices.SendSmsService.LoginCodeForUserInWebsiteOne(RegisterSeller.PhoneNumber, phoneNumberToken);
                user.SendSmsLastTime = DateTime.Now;

            }

            await _userManager.UpdateAsync(user);
            await _uow.SaveChangesAsync();

            return RedirectToPage("./ConfirmationPhoneNumber", new { phoneNumber = RegisterSeller.PhoneNumber });
        }
    }
}
