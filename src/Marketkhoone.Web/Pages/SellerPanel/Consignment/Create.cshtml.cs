using System.ComponentModel.DataAnnotations;
using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Consignments;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.SellerPanel.Consignment
{
    public class CreateModel : SellerPanelBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeFacadeServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IViewRendererService _viewRendererService;

        public CreateModel(IFacadeServices facadeFacadeServices, IMapper mapper,
            IUnitOfWork uow, IViewRendererService viewRendererService)
        {
            _facadeFacadeServices = facadeFacadeServices;
            _mapper = mapper;
            _uow = uow;
            _viewRendererService = viewRendererService;
        }

        #endregion
        [Display(Name="کد تنوع")]
        [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
        [Range(1, int.MaxValue, ErrorMessage = AttributesErrorMessages.RegularExpressionMessage)]
        public int VariantCode { get; set; }

        public CreateConsignmentViewModel CreateConsignment { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(CreateConsignmentViewModel createConsignment)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            if (createConsignment.Variants.Count < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var deliveryDate = createConsignment.DeliveryDate.ToGregorianDate();
            if (!deliveryDate.IsSuccessful)
            {
                return Json(new JsonResultOperation(false));
            }

            // 1|||50
            // 2|||30

            var variantCodes = new List<int>();
            foreach (var variantCode in createConsignment.Variants)
            {
                // براساس سه تا پای ایسپیلیت کن
                // خونه صفر یعنی اون 1 و 2 رو بریزت تو
                //variantCodeToAdd
                var splitVariant = variantCode.Split("|||");
                if (!int.TryParse(splitVariant[0], out var variantCodeToAdd))
                {
                    return Json(new JsonResultOperation(false));
                }
                //مقادیر ایسپیلیت شده رو اد کن توی
                //varaintCodeToAdd
                variantCodes.Add(variantCodeToAdd);
            }

            //رکورد  تکراری نباید دداخل کد های تنوع وجود داشته باشد
            //10
            //8
            //همون داستان ایندکس ها در اینتیتیه
            //ConsignmentItems
            if (createConsignment.Variants.Count != createConsignment.Variants.Distinct().Count())
            {
                return Json(new JsonResultOperation(false));
            }

            var consignmentToAdd = new MarketKhoone.Entities.Consignment
            {
                DeliveryDate = deliveryDate.Result,
                SellerId = await _facadeFacadeServices.SellerService.GetSellerId()
            };

            //تنوع های اومده از ظرف فروشنده رو از پایگاه داده میخونیم
            // آیدی برای استفاده در جدول آیتم های محموله
            //کدتنوع هم برای جستجو در داخل ورودی ها کاربر که بقهمیم
            //count
            //وارد شده هر محموله چه تعداد است

            var productVariants = await _facadeFacadeServices.ProductVariantService
                .GetProductVariantsForCreateConsignment(variantCodes);


            //به همان تعداد که تنوع از طرف کاربر اومده
            //باید به همان تعداد تنوع رو از پایگاه داده بخونیم

            if (productVariants.Count != variantCodes.Count)
            {
                return Json(new JsonResultOperation(false));
            }

            foreach (var productVariant in productVariants)
            {
                // 1|||20  از تنوع یک، بیست تا اومده

                var variantCodeToCompare = $"{productVariant.VariantCode} |||";
                var variantItem = createConsignment.Variants
                    .Single(x => x.StartsWith(variantCodeToCompare));
                var productCountString = variantItem.Split("|||")[1];
                if (!int.TryParse(productCountString, out var productCount))
                {
                    return Json(new JsonResultOperation(false));
                }

                var maxProductCount = 100000;
                if (productCount > maxProductCount)
                {
                    return Json(new JsonResultOperation(false, $"تعداد هر محصول باید بین 1 تا {maxProductCount}"));
                }
                //پس باید در نویگیشن نمنه سازی کنیم
                consignmentToAdd.ConsignmentItems.Add(new ConsignmentItem()
                {
                    //چون داریم از خود
                    //Consignment
                    //اقدام میکنیم
                    //ConsignmentId = consignmentToAdd.Id
                    Count = productCount,
                    ProductVariantId = productVariant.Id,
                    Barcode = $"{productVariant.Id} -- {consignmentToAdd.SellerId}"
                });
            }

            await _facadeFacadeServices.ConsignmentService.AddAsync(consignmentToAdd);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "محموله مورد نظر ایجاد شد")
            {
                Data = Url.Page("ConfirmationConsignment")
            });
        }

        public async Task<IActionResult> OnPostGetConsignmentTr(int variantCode)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });

            }

            var productVariant =
                await _facadeFacadeServices.ProductVariantService.GetProductVariantForCreateConsignment(variantCode);
            if (productVariant is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Json(new JsonResultOperation(true, string.Empty)
            {
                //همیشه اینجا آدرس رو کامل وارد کن
                //دیتا رو به 
                //String
                //تبدیل میکنه و در قالب 
                //Html
                //به ما برگشت میده
                Data = await _viewRendererService.RenderViewToStringAsync(
                    "~/Pages/SellerPanel/Consignment/_ProductVariantTrPartial.cshtml",
                    productVariant)

                //توی 
                //Site.js
                //نوشتیم:
                // window[functionName](data.data);
                //data
                //دوم
                //همون
                //Data
                //ی اینجاست.
            });
        }
    }
}
