using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Parbad;

namespace Marketkhoone.Web.Pages.Profile.Wallet
{
    public class VerifyPaymentModel : PageModel
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IOnlinePayment _onlinePayment;
        private readonly IUnitOfWork _uow;

        public VerifyPaymentModel(IFacadeServices facadeServices,
            IOnlinePayment onlinePayment, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _onlinePayment = onlinePayment;
            _uow = uow;
        }

        #endregion

        public async Task<IActionResult> OnGet()
        {
            return await Verify();
        }

        public async Task<IActionResult> OnPost()
        {
            return await Verify();
        }
        /// <summary>
        /// تایید کردن پرداخت کاربر
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Verify()
        {
            var invoice = await _onlinePayment.FetchAsync();

            if (invoice.Status != PaymentFetchResultStatus.ReadyForVerifying)
            {
                // Check if the invoice is new or it's already processed before.
                var isAlreadyProcessed = invoice.Status == PaymentFetchResultStatus.AlreadyProcessed;
                var isAlreadyVerified = invoice.IsAlreadyVerified;
                return Content("The payment was not successful");
            }

            var verifyResult = await _onlinePayment.VerifyAsync(invoice);

            // checking the status of the verification method
            if (verifyResult.Status != PaymentVerifyResultStatus.Succeed)
            {
                //check if the payment is already verified
                var isAlreadyVerified = verifyResult.Status == PaymentVerifyResultStatus.AlreadyVerified;
                return Content("The payment is already verified before");
            }

            var userId = User.Identity.GetLoggedInUserId();
            var wallet = await _facadeServices.WalletService.FindByTrackingNumber(verifyResult.TrackingNumber, userId);
            if (wallet is null)
            {
                return Content("The payment was not successful.");
            }

            wallet.BankTransactionCode = verifyResult.TransactionCode;
            wallet.IsPay = true;
            await _uow.SaveChangesAsync();

            return Content("The Payment was successful");
        }
    }
}
