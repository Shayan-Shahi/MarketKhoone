using EShopMarket.Common.Helpers;
using Ganss.Xss;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Enums.Consignment;
using MarketKhoone.Entities.Enums.Product;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Consignments;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Inventory.Consignment
{
    public class IndexModel : InventoryPanelBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeService;
        private readonly IUnitOfWork _uow;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public IndexModel(IFacadeServices facadeService, IUnitOfWork uow, IHtmlSanitizer htmlSanitizer)
        {
            _facadeService = facadeService;
            _uow = uow;
            _htmlSanitizer = htmlSanitizer;
        }

        #endregion

        [BindProperty(SupportsGet = true)]
        public ShowConsignmentsViewModel Consignments { get; set; } = new();
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

            return Partial("List", await _facadeService.ConsignmentService.GetConsignments(Consignments));
        }

        public async Task<IActionResult> OnGetGetConsignmentDetails(long consignmentId)
        {
            if (consignmentId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var consignmentDetails = await _facadeService.ConsignmentService.GetConsignmentDetails(consignmentId);

            if (consignmentDetails.Items.Count < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            return Partial("ConsignmentDetailsPartial", consignmentDetails);
        }

        public async Task<IActionResult> OnPostConfirmationConsignment(long consignmentId)
        {
            if (consignmentId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var consignment = await _facadeService.ConsignmentService.GetConsignmentForConfirmation(consignmentId);
             
            if (consignment is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            consignment.ConsignmentStatus = ConsignmentStatus.ConfirmAndAwaitingForConsignment;
            await _uow.SaveChangesAsync();
            //todo: Send Email to the Seller
            return Json(new JsonResultOperation(true,
                "محموله مورد نظر با موفقیت تایید شده و در انتظار دریافت توسط فروشنده قرار گرفت"));
        }

        public async Task<IActionResult> OnGetAutoCompleteSearch(string term)
        {
            return Json(await _facadeService.SellerService.GetShopNamesForAutocomplete(term));
        }

        public async Task<IActionResult> OnPostReceiveConsignment(long consignmentId)
        {
            if (consignmentId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var consignment =
                await _facadeService.ConsignmentService.GetConsignmentToChangesStatusToReceived(consignmentId);

            if (consignment is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            consignment.ConsignmentStatus = ConsignmentStatus.Received;
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true,
                "محموله مورد نظر با موفیت دریافت شد، لظفا موجودی کالاها را افزایش دهید"));

        }

        public async Task<IActionResult> OnGetChangeConsignmentStatus(long consignmentId)
        {
            if (consignmentId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            if (!await _facadeService.ConsignmentService.IsExistsConsignmentWithReceivedStatus(consignmentId))
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var model = new AddDescriptionForConsignmentViewModel()
            {
                ConsignmentId = consignmentId
            };

            return Partial("ChangeConsignmentStatusToReceivedAndAddStockPartial", model);
        }

        public async Task<IActionResult> OnPostChangeConsignmentStatusToReceivedAndAddStock(
            AddDescriptionForConsignmentViewModel model)
        {
            //چرا این قسمتو کامنت کردیم؟
            //چون در ویوو مدل برای آیدی اتریبیوت
            //[Range()]
            //رو استفاده کردیم که همین کار رو میکنیم

            //if (model.ConsignmentId < 1)
            //{
            //    return Json(new JsonResultOperation(false));
            //}
            var consignment = await _facadeService.ConsignmentService.GetConsignmentWithReceivedStatus(
                model.ConsignmentId);

            if (consignment is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            consignment.ConsignmentStatus = ConsignmentStatus.ReceivedAndAddStock;
            consignment.Description = _htmlSanitizer.Sanitize(model.Description);
            #region AddStock

            var productStocks =
                await _facadeService.ProductStockService.GetProductStocksForAddProductVariantsCount(model.ConsignmentId);

            var productVariantIds = productStocks.Select(x => x.Key).ToList();
            var productVariants =
                await _facadeService.ProductVariantService.GetProductVariantsToAddCount(productVariantIds);


            foreach (var productStock in productStocks)
            {
                var productVariant = productVariants.SingleOrDefault(x => x.Id == productStock.Key);
                if (productVariant is not null)
                {
                    //اگه از قبل چنین چیزی وجود داشت
                    //اضافه بکن
                    productVariant.Count += productStock.Value;
                }
            }

            #endregion

            //وضعیت محصول هایی که در حالت ناموجود هستند
            //اونارو در حالت موجود قزار میدیهیم
            //جون موجودی اون ها افزایش پیدا کرده است

            //جرا رکورد هار تکراری رو با متد
            //Distinct
            //حذف میکنیم
            //جون اممان داره که در یک محموله از یک محصول دو تا وجود داشته باشه
            // رنگ آبی برای گوشی ایکس
            //رنگ قرمز برای گوشی ایکس

            var productIds = productVariants.Select(x => x.ProductId).Distinct().ToList();
            var productsToChangeTheirStatus =
                await _facadeService.ProductService.GetProductsForChangeStatus(productIds);
            foreach (var product in productsToChangeTheirStatus)
            {
                if (product.ProductStockStatus == ProductStockStatus.Unavailable)
                {
                    product.ProductStockStatus = ProductStockStatus.Available;
                }
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "موجودی کالاها با موفقیت افزایش یافت"));

        }
    }
}
