using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Profile.Adresses
{
    public class IndexModel : PageBase
    {

        #region Constructor
        private readonly IFacadeServices _facadeService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public IndexModel(IFacadeServices facadeService, IUnitOfWork uow, IMapper mapper)
        {
            _facadeService = facadeService;
            _uow = uow;
            _mapper = mapper;
        }
        #endregion

        public List<ShowAddressInProfileViewModel> Addresses { get; set; }
        public async Task OnGet()
        {
            Addresses = await _facadeService.AddressService.GetAllUserAddresses();
        }

        public async Task<IActionResult> OnPostDeleteAddress(long id)
        {
            if (!await _facadeService.AddressService.RemoveUserAddress(id))
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "آدرس مورد نظر با موفقیت حذف شد"));
        }


        public async Task<IActionResult> OnGetEditAddress(long id)
        {
            var addressToEdit = await _facadeService.AddressService.GetForEdit(id);

            if (addressToEdit is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("Edit", addressToEdit);
        }

        public async Task<IActionResult> OnPostEdit(EditAddressInProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            var addressToEdit = await _facadeService.AddressService.FindByIdAsync(model.Id);
            if (addressToEdit is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }


            addressToEdit = _mapper.Map(model, addressToEdit);


            await _facadeService.AddressService.Update(addressToEdit);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "آدرس مورد نظر با موفقیت ویرایش شد"));
        }
        public IActionResult OnGetAdd()
        {
            return Partial("Add");
        }
        public async Task<IActionResult> OnGetAutocompleteSearchForProvince(string term)
        {
            return Json(await _facadeService.ProvinceAndCityService.GetProvinceForAutocomplete(term));
        }
    }
}
