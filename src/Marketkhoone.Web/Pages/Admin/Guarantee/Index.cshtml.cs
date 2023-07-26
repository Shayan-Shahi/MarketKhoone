using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Brands;
using MarketKhoone.ViewModels.Guarantees;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Admin.Guarantee
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
        public ShowGuaranteesViewModel Guarantees { get; set; } = new();
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
            return Partial("List", await _facadeServices.GuaranteeService.GetGuarantees(Guarantees));
        }


        public IActionResult OnGetAdd()
        {
            return Partial("Add");
        }
        public async Task<IActionResult> OnPostAdd(AddGuaranteeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            var guarantee = _mapper.Map<MarketKhoone.Entities.Guarantee>(model);
            guarantee.IsConfirmed = true;

            string guaranteePictureFileName = null;
            if (model.Picture.IsFileUploaded())
                guaranteePictureFileName = model.Picture.GenerateFileName();
            guarantee.Picture = guaranteePictureFileName;

            var result = await _facadeServices.GuaranteeService.AddAsync(guarantee);
            if (!result.Ok)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.DuplicateErrorMessage)
                {
                    Data = result.Columns.SetDuplicateColumnsErrorMessages<AddBrandViewModel>()
                });
            }
            await _uow.SaveChangesAsync();
            await _facadeServices.UploadedFileService.SaveFile(model.Picture, guarantee.Picture, null, "images", "guarantees");
            return Json(new JsonResultOperation(true, "گارانتی مورد نظر با موفقیت اضافه شد"));
        }

        public async Task<IActionResult> OnGetEdit(long id)
        {
            var model = await _facadeServices.GuaranteeService.GetForEdit(id);

            if (model is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("Edit", model);
        }

        public async Task<IActionResult> OnPostEdit(EditGuaranteeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            var guaranteeToUpdate = await _facadeServices.GuaranteeService.FindByIdAsync(model.Id);
            if (guaranteeToUpdate is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var oldPictureFileName = guaranteeToUpdate.Picture;


            guaranteeToUpdate = _mapper.Map(model, guaranteeToUpdate);

            string PictureFileName = null;
            if (model.NewPicture.IsFileUploaded())
                PictureFileName = model.NewPicture.GenerateFileName();
            guaranteeToUpdate.Picture = PictureFileName;

            await _facadeServices.GuaranteeService.Update(guaranteeToUpdate);
            await _uow.SaveChangesAsync();
            await _facadeServices.UploadedFileService.SaveFile(model.NewPicture, guaranteeToUpdate.Picture, oldPictureFileName, "images", "guarantees");
            return Json(new JsonResultOperation(true, "گارانتی مورد نظر با موفقیت ویرایش شد"));
        }

        public async Task<IActionResult> OnGetGuaranteeDetails(long brandId)
        {
            var model = await _facadeServices.GuaranteeService.GetGuaranteeDetails(brandId);
            if (model is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("Details", model);
        }

        public async Task<IActionResult> OnPostConfirmGuarantee(long id)
        {
            var guarantee = await _facadeServices.GuaranteeService.GetInActiveGuarantee(id);
            if (guarantee is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            guarantee.IsConfirmed = true;
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "گارانتی مورد نظر با موفقیت تایید شد"));
        }
    }
}

