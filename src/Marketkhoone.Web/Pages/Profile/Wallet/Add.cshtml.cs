using AutoMapper;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Enums.Order;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Wallets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Gateway.ZarinPal;


namespace Marketkhoone.Web.Pages.Profile.Wallet
{
    public class AddModel : PageModel
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IMapper _mapper;
        private readonly IOnlinePayment _onlinePayment;
        private readonly IUnitOfWork _uow;

        public AddModel(IFacadeServices facadeServices, IMapper mapper,
            IOnlinePayment onlinePayment, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _mapper = mapper;
            _onlinePayment = onlinePayment;
            _uow = uow;
        }

        #endregion


        [BindProperty]
        public AddValueToWalletViewModel AddValeToWallet { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //کاربر بعد از پرداخت به چه آدرسی باید هدایت شود
            var callBackUrl = Url.PageLink("VerifyPayment", null, null, Request.Scheme);

            var walletToAdd = _mapper.Map<MarketKhoone.Entities.Wallet>(AddValeToWallet);

            walletToAdd.UserId = User.Identity.GetLoggedInUserId();
            walletToAdd.Description = "افزایش موجودی توسط کاربر";

            var result = await _onlinePayment.RequestAsync(invoice =>
            {
                invoice
                    .SetAmount(AddValeToWallet.Value)
                    .SetCallbackUrl(callBackUrl)
                    .SetGateway(AddValeToWallet.PaymentGateway.ToString())
                    .UseAutoIncrementTrackingNumber();
                if (AddValeToWallet.PaymentGateway == PaymentGateway.Zarinpal)
                {
                    invoice.SetZarinPalData(new ZarinPalInvoice("Not description"));
                }
            });

            walletToAdd.TrackingNumber = result.TrackingNumber;

            if (result.IsSucceed)
            {
                await _facadeServices.WalletService.AddAsync(walletToAdd);
                await _uow.SaveChangesAsync();

                return result.GatewayTransporter.TransportToGateway();
            }
            else
            {
                return RedirectToPage(PublicConstantStrings.Error500PageName);
            }
        }
    }
}
