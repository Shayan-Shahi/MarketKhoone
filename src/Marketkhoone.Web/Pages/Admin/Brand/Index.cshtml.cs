using AutoMapper;
using EShopMarket.Common.Helpers;
using Ganss.Xss;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Brands;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Admin.Brand
{
    public class IndexModel : PageBase
    {

        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public IndexModel(IFacadeServices facadeServices, IMapper mapper, IUnitOfWork uow, IHtmlSanitizer htmlSanitizer)
        {
            _facadeServices = facadeServices;
            _mapper = mapper;
            _uow = uow;
            _htmlSanitizer = htmlSanitizer;
        }
        #endregion


        [BindProperty(SupportsGet = true)]
        public ShowBrandsViewModel Brands { get; set; } = new();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnGetGetDataTableAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, PublicConstantStrings.ModelStateErrorMessage);
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            return Partial("List", await _facadeServices.BrandService.GetBrands(Brands));
        }

        public IActionResult OnGetAdd()
        {
            return Partial("Add");
        }

        public async Task<IActionResult> OnPostAddAsync(AddBrandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            var brand = _mapper.Map<MarketKhoone.Entities.Brand>(model);
            brand.Slug = brand.TitleEn.ToUrlSlug();

            brand.Description = _htmlSanitizer.Sanitize(brand.Description);
            brand.IsConfirmed = true;

            string brandLogoFileName = null;
            if (model.LogoPicture.IsFileUploaded())
                brandLogoFileName = model.LogoPicture.GenerateFileName();
            brand.LogoPicture = brandLogoFileName;

            string brandRegistrationFileName = null;
            if (model.BrandRegistrationPicture.IsFileUploaded())
                brandRegistrationFileName = model.BrandRegistrationPicture.GenerateFileName();
            brand.BrandRegistrationPicture = brandRegistrationFileName;


            var result = await _facadeServices.BrandService.AddAsync(brand);
            if (!result.Ok)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.DuplicateErrorMessage)
                {
                    Data = result.Columns.SetDuplicateColumnsErrorMessages<AddBrandViewModel>()
                });
            }

            await _uow.SaveChangesAsync();
            await _facadeServices.UploadedFileService.SaveFile(model.LogoPicture, brandLogoFileName, null, "images", "brandsLogo");
            await _facadeServices.UploadedFileService.SaveFile(model.BrandRegistrationPicture, brandRegistrationFileName, null, "images", "brandRegistrationPicture");


            return Json(new JsonResultOperation(true, "برند مورد نظر با موفقیت اضافه شد"));
        }

        public async Task<IActionResult> OnGetCheckForTitleFa(string titleFa)
        {
            return Json(!await _facadeServices.BrandService.IsExistsBy(nameof(MarketKhoone.Entities.Brand.TitleFa), titleFa));
        }

        public async Task<IActionResult> OnGetCheckForTitleEn(string titleEn)
        {
            return Json(!await _facadeServices.BrandService.IsExistsBy(nameof(MarketKhoone.Entities.Brand.TitleEn), titleEn));
        }

        public async Task<IActionResult> OnGetCheckForTitleFaOnEdit(string titleFa, long id)
           {
            return Json(!await _facadeServices.BrandService.IsExistsBy(nameof(MarketKhoone.Entities.Brand.TitleFa), titleFa, id));
        }

        public async Task<IActionResult> OnGetCheckForTitleEnOnEdit(string titleEn, long id)
        {
            return Json(!await _facadeServices.BrandService.IsExistsBy(nameof(MarketKhoone.Entities.Brand.TitleEn), titleEn, id));
        }

        public async Task<IActionResult> OnGetEdit(long id)
        {
            var model = await _facadeServices.BrandService.GetForEdit(id);
            if (model is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }
            return Partial("Edit", model);
        }

        public async Task<IActionResult> OnPostEdit(EditBrandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            var brandToUpdate = await _facadeServices.BrandService.FindByIdAsync(model.Id);
            if (brandToUpdate is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            brandToUpdate.Slug = model.TitleEn.ToUrlSlug();

            var oldLogoPictureFileName = brandToUpdate.LogoPicture;
            var oldBrandRegistrationFileName = brandToUpdate.BrandRegistrationPicture;

            brandToUpdate = _mapper.Map(model, brandToUpdate);

            string logoPictureFileName = null;
            if (model.NewLogoPicture.IsFileUploaded())
                logoPictureFileName = model.NewLogoPicture.GenerateFileName();
            brandToUpdate.LogoPicture = logoPictureFileName;

            string brandRegistrationFileName = null;
            if (model.NewBrandRegistrationPicture.IsFileUploaded())
                brandRegistrationFileName = model.NewBrandRegistrationPicture.GenerateFileName();
            brandToUpdate.BrandRegistrationPicture = brandRegistrationFileName;

            await _facadeServices.BrandService.Update(brandToUpdate);
            await _uow.SaveChangesAsync();
            await _facadeServices.UploadedFileService.SaveFile(model.NewLogoPicture, brandToUpdate.LogoPicture, oldLogoPictureFileName, "images", "brandsLogo");
            await _facadeServices.UploadedFileService.SaveFile(model.NewBrandRegistrationPicture, brandToUpdate.BrandRegistrationPicture, oldBrandRegistrationFileName, "images", "brandRegistrationPicture");
            return Json(new JsonResultOperation(true, "برند مورد نظر با موفقیت ویرایش شد"));
        }

        public async Task<IActionResult> OnGetBrandDetails(long brandId)
        {
            var model = await _facadeServices.BrandService.GetBrandDetails(brandId);
            if (model is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            return Partial("BrandDetails", model);
        }

        public async Task<IActionResult> OnPostRejectBrand(BrandDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, "لطفا دلیل رد برند را وارد نمایید"));
            }

            var brand = await _facadeServices.BrandService.GetInActiveBrand(model.Id);
            if (brand is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            brand.IsConfirmed = false;
            await _uow.SaveChangesAsync();
            // todo: Send Email to seller to notify them why they have been rejected.
            //_facadeServices.EmailService.SendEmail(model.SellerUserEmail, "رد برند", model.RejectReason);
            return Json(new JsonResultOperation(true, "برند مورد نظر با موفقیت رد شد."));

        }

        public async Task<IActionResult> OnPostRestoreAsync(long elementId)
        {
            var brandToRestore = await _facadeServices.BrandService.FindByIdAsync(elementId);
            if (brandToRestore is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }
            _facadeServices.BrandService.Restore(brandToRestore);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "دسته بندی مورد نظر با موفقیت بازگردانی شد"));
        }

        public async Task<IActionResult> OnPostConfirmBrand(long id)
        {
            var brand = await _facadeServices.BrandService.GetInActiveBrand(id);
            if (brand is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            brand.IsConfirmed = true;
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "برند مورد نظر با موفقیت تایید شد"));
        }

        public async Task<IActionResult> OnPostDeleteBrand(long id)
        {
            if (id < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var brandToRemove = await _facadeServices.BrandService.FindByIdAsync(id);
            if (brandToRemove is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            // brandToRemove.IsDeleted = true;
            //or
            _facadeServices.BrandService.SoftDelete(brandToRemove);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "برند مورد نظر با موفقیت حذف شد."));
        }


      
    }
}
