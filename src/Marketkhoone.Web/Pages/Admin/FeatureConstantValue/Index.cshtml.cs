using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.FeatureConstantValues;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Admin.FeatureConstantValue
{
    public class IndexModel : PageBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public IndexModel(IFacadeServices facadeServices, IUnitOfWork uow, IMapper mapper)
        {
            _facadeServices = facadeServices;
            _uow = uow;
            _mapper = mapper;
        }

        #endregion

        [BindProperty(SupportsGet = true)]
        public ShowFeatureConstantValuesViewModel FeatureConstantValues { get; set; } = new();
        public async Task OnGet()
        {
            var categories = await _facadeServices.CategoryService.GetCategoriesWithNoChild();
            FeatureConstantValues.SearchFeatureConstantValues.Categories =
                categories.CreateSelectListItem(firstItemValue: string.Empty);
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

            return Partial("List", await _facadeServices.FeatureConstantValueService.GetFeatureConstantValues(FeatureConstantValues));
        }

        public async Task<IActionResult> OnGetGetCategoryFeatures(long categoryId)
        {
            //جرا صفر؟
            // چون موقعی که کاربر خودش میاد و سلکت باکس رو
            //روی انتخاب کنید میزاره
            // مقدار ولیوش صفر میشه و ون آیدی بصورت لانگ غیر نال ایبیل هست، صفر میاد سمت اکشن
            if (categoryId < 0)
            {
                return Json(new JsonResultOperation(false));
            }

            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = await _facadeServices.CategoryFeatureService.GetCategoryFeatures(categoryId)
            });
        }

        public async Task<IActionResult> OnPostDelete(long featureConstantValueId)
        {
            if (featureConstantValueId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var featureConstantValue =
                await _facadeServices.FeatureConstantValueService.FindByIdAsync(featureConstantValueId);

            if (featureConstantValue is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            _facadeServices.FeatureConstantValueService.Remove(featureConstantValue);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "مقدار ثابت ویژگی مورد نظر با موفقیت حذف شد"));
        }

        public async Task<IActionResult> OnGetAdd()
        {
            //در لودینگ مودال، در همون ابتدا لودینگ
            //یه سلکت باکس داریم که همه دسته بندی ها رو توش پر میکنیم
            // تا کاربر بیاد و انتخاب کنه و....
            var categories = await _facadeServices.CategoryService.GetLastCategoriesWithNoChild();
            var model = new AddFeatureConstantValueViewModel()
            {
                Categories = categories.CreateSelectListItem()
            };
            return Partial("Add", model);
        }

        public async Task<IActionResult> OnPostAdd(AddFeatureConstantValueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            //آیا مقدار ثابتی که کاربر داره ه ویژگی فلان از فلان دسته بندی میده
            //واقعا متعلق به اون دسته بندی و اون ویژگی هست
            // مثلا رم 16 گیگ رو برای دسته بندی گوشی موبایل باید بفرسته به دسته بندی چادر مسافرتی نفرسته یه موقع

            if (!await _facadeServices.CategoryFeatureService.CheckCategoryFeature(model.CategoryId, model.FeatureId))
            {
                return Json(new JsonResultOperation(false));
            }

            var featureConstantValueToAdd = _mapper.Map<MarketKhoone.Entities.FeatureConstantValue>(model);
            await _facadeServices.FeatureConstantValueService.AddAsync(featureConstantValueToAdd);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "مقدار ویژگی ثابت مورد نظر یا موفقیت اضافه شد"));
        }

        public async Task<IActionResult> OnGetEdit(long id)
        {
            var featureConstantValue = await _facadeServices.FeatureConstantValueService.FindByIdAsync(id);
            if (featureConstantValue is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var categories = await _facadeServices.CategoryService.GetCategoriesToShowInSelectBoxAsync();
            var features =
                await _facadeServices.CategoryFeatureService.GetCategoryFeatures(featureConstantValue.CategoryId);

            var model = _mapper.Map<EditFeatureConstantValueViewModel>(featureConstantValue);
            model.Categories = categories.CreateSelectListItem();
            //در اکستنش سلکت لیست آیتمی که ما نوشتیم
            //ویوو مدلی که توسط آن سلکت لیست آیتم پر میشه
            //باید یک 
            //Id, title
            //داشته باشد
            // ولی در ویو مدل این قسمت یعنی 
            //CategoryFeatureForCreateProductViewModel => به متد GetCategoryFeatures نگاه کن که همین ویوو مدل رو گرفته
            //FeatureId, FeatureTitle
            //داریم پس باید به اکستنشن متد بگیم منظور ما از آیدی و تایتل چیه
            model.Features = features.CreateSelectListItem(idPropertyName: "FeatureId", titlePropertyName: "FeatureTitle");
            return Partial("Edit", model);
        }

        public async Task<IActionResult> OnPostEdit(EditFeatureConstantValueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            if (!await _facadeServices.CategoryFeatureService.CheckCategoryFeature(model.CategoryId, model.FeatureId))
            {
                return Json(new JsonResultOperation(false));
            }

            var featureConstantValue = await _facadeServices.FeatureConstantValueService.FindByIdAsync(model.Id);
            if (featureConstantValue is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            featureConstantValue = _mapper.Map(model, featureConstantValue);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "مقدار ثابت ویژگی مورد نظر با موفقیت ویرایش شد"));
        }


    }
}
