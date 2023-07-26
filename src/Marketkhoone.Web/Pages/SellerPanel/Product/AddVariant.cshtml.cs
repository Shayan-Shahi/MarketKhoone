using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities.Enums.Product;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Variants;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.SellerPanel.Product
{
    public class AddVariantModel : SellerPanelBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public AddVariantModel(IFacadeServices facadeServices, IMapper mapper, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _mapper = mapper;
            _uow = uow;
        }

        #endregion

        [BindProperty]
        public AddVariantViewModel Variant { get; set; }
        public async Task<IActionResult> OnGet(long productId)
        {
            var productInfo = await _facadeServices.ProductService.GetProductInfoForAddVariant(productId);

            if (productInfo is null)
            {
                return RedirectToPage(PublicConstantStrings.Error404PageName);
            }

            Variant = productInfo;
            return Page();

        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            var checkInputs =
                await _facadeServices.VariantService.CheckProductAndVariantTypeForAddVariant(Variant.ProductId,
                    Variant.VariantId);

            if (!checkInputs.isISuccessful)
            {
                return Json(new JsonResultOperation(false));
            }

            var productVariantToAdd = _mapper.Map<MarketKhoone.Entities.ProductVariant>(Variant);


            if (checkInputs.IsVariantNull)
            {
                productVariantToAdd.VariantId = null;
            }
            //نبای اجازه بدهیم که یک فروشنده دوبا یک تنوع رو اضافه کند
            if (await _facadeServices.ProductVariantService.IsThisVariantAddedForSeller(productVariantToAdd.VariantId,
                    productVariantToAdd.ProductId))
            {
                return Json(new JsonResultOperation(false));
            }

            productVariantToAdd.VariantCode =
                await _facadeServices.ProductVariantService.GetVariantCodeForCreateProductVariant();


            // Get SellerId for Entity

            var userId = User.Identity.GetLoggedInUserId();
            var sellerId = await _facadeServices.SellerService.GetSellerId(userId);
            productVariantToAdd.SellerId = sellerId;

            await _facadeServices.ProductVariantService.AddAsync(productVariantToAdd);

            //وضعیت محصول اگر جدید باشد
            //یعنی به تازگی ایجاد شده باشد و تنوعی نداشته باشه
            // اگر تنوعی براش اصافه بشه باید وضعیت محصول را در حالت ناموجود قزار بدهیم

            var product = await _facadeServices.ProductService.FindByIdWithIncludesAsync
                (Variant.ProductId, nameof(MarketKhoone.Entities.Product.Category));

            if (product.ProductStockStatus == ProductStockStatus.New)
            {
                product.ProductStockStatus = ProductStockStatus.Unavailable;
            }

            product.Category.HasVariant = true;

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "تنوع محصول با موفقیت اضافه شد")
            {
                Data = Url.Page("SuccessfulProductVariant")
            });
        }

        public async Task<IActionResult> OnGetGetGuarantees(string input)
        {
            var result = await _facadeServices.GuaranteeService
                .SearchOnGuaranteesForSelect2(input);

            var specificGuarantee = result.Select((value, index)
                => new { value, index }).SingleOrDefault(p => p.value.Text.Contains("0 ماهه"));

            if (specificGuarantee != null)
            {
                result[specificGuarantee.index] = new ShowSelect2DataByAjaxViewModel()
                {
                    Text = "گارانتی اصالت و سلامت فیزیکی کالا",
                    Id = specificGuarantee.value.Id
                };
            }

            return Json(new
            {
                results = result
            });
        }


    }
}
