using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Brands;
using MarketKhoone.ViewModels.Variants;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Admin.Variant
{
    public class IndexModel : PageBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public IndexModel(IFacadeServices facadeServices, IMapper mapper, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _mapper = mapper;
            _uow = uow;
        }

        #endregion

        [BindProperty(SupportsGet = true)]
        public ShowVariantsViewModel Variants { get; set; } = new();
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
            return Partial("List", await _facadeServices.VariantService.GetVariants(Variants));
        }

        public IActionResult OnGetAdd()
        {
            return Partial("Add");
        }

        public async Task<IActionResult> OnPostAddAsync(AddVariantByAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            var variant = _mapper.Map<MarketKhoone.Entities.Variant>(model);
            if (model.Value.IsNumeric())
            {
                variant.IsColor = false;
            }
            else
            {
                variant.IsColor = true;
            }
            
            variant.IsConfirmed = true;


            var result = await _facadeServices.VariantService.AddAsync(variant);
            if (!result.Ok)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.DuplicateErrorMessage)
                {
                    Data = result.Columns.SetDuplicateColumnsErrorMessages<AddVariantByAdminViewModel>()
                });
            }

            await _uow.SaveChangesAsync();

            return Json(new JsonResultOperation(true, "تنوع مورد نظر با موفقیت اضافه شد"));
        }
    }
}
