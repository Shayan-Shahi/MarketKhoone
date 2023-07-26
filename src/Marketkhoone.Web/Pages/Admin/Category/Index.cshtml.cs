using AutoMapper;
using EShopMarket.Common.Helpers;
using Ganss.Xss;
using MarketKhoone.Common;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Categories;
using MarketKhoone.ViewModels.CategoryVariants;
using Microsoft.AspNetCore.Mvc;


namespace Marketkhoone.Web.Pages.Admin.Category
{

    public class IndexModel : PageBase
    {

        #region Constructor
        private readonly IFacadeServices _facadeServices;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public IndexModel(IFacadeServices facadeServices, IUnitOfWork uow, IMapper mapper, IHtmlSanitizer htmlSanitizer)
        {
            _facadeServices = facadeServices;
            _uow = uow;
            _mapper = mapper;
            _htmlSanitizer = htmlSanitizer;
        }
        #endregion
        [BindProperty(SupportsGet = true)]
        public ShowCategoriesViewModel Categories { get; set; } = new();
        public void OnGet()
        {
        }


        public async Task<IActionResult> OnGetGetDataTableAsync(ShowCategoriesViewModel categories)
        {
            //Thread.Sleep(1000);
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            return Partial("List", await _facadeServices.CategoryService.GetCategories(categories));
        }

        public async Task<IActionResult> OnGetAdd(long id = 0)
        {
            if (id > 0)
            {
                if (!await _facadeServices.CategoryService.IsExistsBy(nameof(MarketKhoone.Entities.Category.Id), id))
                {
                    return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
                }
            }

            var categories = await _facadeServices.CategoryService.GetCategoriesToShowInSelectBoxAsync();
            var model = new AddCategoryViewModel()
            {

                ParentId = id,
                MainCategories = categories.CreateSelectListItem(firstItemText: "خودش دسته اصلی باشد")

            };
            return Partial("Add", model);
        }

        public async Task<IActionResult> OnPostAdd(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            var category = _mapper.Map<MarketKhoone.Entities.Category>(model);
            category.Description = _htmlSanitizer.Sanitize(category.Description);
            //var category = new MarketKhoone.Entities.Category
            //{
            //    Description = model.Description,
            //    ShowInMenus = model.ShowInMenus,
            //    Title = model.Title,
            //    Slug = model.Slug,
            //};
            string pictureFileName = null;
            if (model.Picture.IsFileUploaded())
                pictureFileName = model.Picture.GenerateFileName();

            if (model.ParentId is 0)
                category.ParentId = null;

            category.Picture = pictureFileName;
            var result = await _facadeServices.CategoryService.AddAsync(category);
            if (!result.Ok)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.DuplicateErrorMessage)
                {
                    Data = result.Columns.SetDuplicateColumnsErrorMessages<AddCategoryViewModel>()
                });
            }

            await _uow.SaveChangesAsync();

            await _facadeServices.UploadedFileService.SaveFile(model.Picture, pictureFileName, null, "images", "categories");
            return Json(new JsonResultOperation(true, "دسته بندی مورد نظر با موفقیت اضافه شد"));
        }

        public async Task<IActionResult> OnGetEdit(long id)
        {
            var model = await _facadeServices.CategoryService.GetForEdit(id);
            if (model is null)
                return Json(new JsonResult(false, PublicConstantStrings.RecordNotFoundMessage));

            var categories = await _facadeServices.CategoryService.GetCategoriesToShowInSelectBoxAsync(id);
            model.MainCategories = categories.CreateSelectListItem(firstItemText: "خودش دسته اصلی باشد");
            return Partial("Edit", model);
        }

        public async Task<IActionResult> OnPostEdit(EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            if (model.Id == model.ParentId)
            {
                return Json(new JsonResultOperation(false, "یک رکورد نمیتواند والد خودش باشد"));
            }
            var category = await _facadeServices.CategoryService.FindByIdAsync(model.Id);


            if (category == null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            string pictureFileName = null;
            if (model.NewPicture.IsFileUploaded())
                pictureFileName = model.NewPicture.GenerateFileName();
            else
            {
                pictureFileName = category.Picture;
            }

            var oldFileName = category.Picture;

            category = _mapper.Map(model, category);
            if (model.ParentId is 0)
                category.ParentId = null;
            category.Picture = pictureFileName;
            var result = await _facadeServices.CategoryService.Update(category);
            if (!result.Ok)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.DuplicateErrorMessage)
                {
                    Data = result.Columns.SetDuplicateColumnsErrorMessages<AddCategoryViewModel>()
                });
            }
            await _uow.SaveChangesAsync();
            await _facadeServices.UploadedFileService.SaveFile(model.NewPicture, pictureFileName, oldFileName, "images", "categories");
            return Json(new JsonResultOperation(true, "دسته بندی مورد نظر با موفقیت ویرایش شد"));
        }
        public async Task<IActionResult> OnPostDeleteAsync(long elementId)
        {
            var category = await _facadeServices.CategoryService.FindByIdAsync(elementId);
            if (category is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }
            _facadeServices.CategoryService.SoftDelete(category);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "دسته بندی مورد نظر با موفقیت حذف شد"));
        }

        public async Task<IActionResult> OnPostDeletePicture(long elementId)
        {
            var category = await _facadeServices.CategoryService.FindByIdAsync(elementId);
            if (category is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var fileName = category.Picture;
            category.Picture = null;
            await _uow.SaveChangesAsync();
            _facadeServices.UploadedFileService.DeleteFile(fileName, "images", "categories");
            return Json(new JsonResultOperation(true, "تصویر دسته بندی مورد نظر با موفقیت حذف شد"));
        }

        public async Task<IActionResult> OnPostRestoreAsync(long elementId)
        {
            var category = await _facadeServices.CategoryService.FindByIdAsync(elementId);
            if (category is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }
            _facadeServices.CategoryService.Restore(category);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "دسته بندی مورد نظر با موفقیت بازگردانی شد"));
        }

        public async Task<IActionResult> OnPostCheckForTitle(string title)
        {
            return Json(!await _facadeServices.CategoryService.IsExistsBy(nameof(MarketKhoone.Entities.Category.Title), title));
        }

        public async Task<IActionResult> OnPostCheckForSlug(string slug)
        {
            return Json(!await _facadeServices.CategoryService.IsExistsBy(nameof(MarketKhoone.Entities.Category.Slug), slug));
        }

        public async Task<IActionResult> OnPostCheckForTitleOnEdit(string title, long id)
        {
            return Json(!await _facadeServices.CategoryService.IsExistsBy(nameof(MarketKhoone.Entities.Category.Title), title, id));
        }

        public async Task<IActionResult> OnPostCheckForSlugOnEdit(string slug, long id)
        {
            return Json(!await _facadeServices.CategoryService.IsExistsBy(nameof(MarketKhoone.Entities.Category.Slug), slug, id));
        }
        public async Task<IActionResult> OnGetAddBrand(long selectedCategoryId)
        {
            //var model = new AddBrandToCategoryViewModel();
            //model.SelectedBrands = await _facadeServices.CategoryService.GetCategoryBrands(selectedCategoryId);
            

            //بصورت 
            //Object initializer
            //کد پایین
            var model = new AddBrandToCategoryViewModel
            {
                SelectedBrands = await _facadeServices.CategoryService.GetCategoryBrands(selectedCategoryId)

                //SelectedCategoryId = categoryId
                //چرا کامنت کردیم؟
                //چون اسم پارامتر ورودی به متد همنام هست با پراپرتی موحود در ویوو مدل

                // در سمت ویوو(لیست) هم باید
                //asp-route-selectedCategoryId =@category.Id
            };
            return Partial("AddBrand", model);

        }

        public async Task<IActionResult> OnPostAddBrand(AddBrandToCategoryViewModel model)
        {
            if (model.SelectedCategoryId < 1)
            {
                return Json(new JsonResultOperation(false));
            }
            //میریم 
            //category
            // ها رو برای این آیدی یا آیدی های اومده بگشت بزنیم تا؟؟؟
            // تا بهش برند ها رو اضاقه کنیم
            //یعنی در اصل قراره 
            //Category و Brand
            //به هم در 
            //CategoryBrand  
            //اضاقه بشن
            // پس باید و باید 
            //CategoryBrand
            //رو هم اینکلود بکنیم و نریم فقط براساس آیدی یا آیدی های اومده، 
            //Category// ها رو برگشت بزنیم
            var selectedCategory = await _facadeServices.CategoryService.GetCategoryWithItsBrands(model.SelectedCategoryId);
            if (selectedCategory is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }
            //همه برند هایی که به دسته بندی ها نسبت داده شده اند
            // طبیعتا مقدارشون در جدول 
            //CategoryBrands
            //هست رو پاک میکنیم تا دوباره از اول اضافه کنیم
            selectedCategory.CategoryBrands.Clear();


            //مبادا کاربر مثلا توسط اینسپکت دوبار برند اپل را بفرسته
            // که در اون صورت سرویس ما هم دو تا آیدی مربوط به برند اپل 
            //به ما برگشت میزنه
            //پس رکورد های تکرار ی رو حذف میکنیم
            model.SelectedBrands = model.SelectedBrands.Distinct().ToList();

            //برندهای ما تکراری نیستند، جون در انتیتی برند ایندکس تایتل رو برای برند یونیک کردیم
            //پس از دیکشنری استفاده میکینیم
            var brandsInDictionary = new Dictionary<string, byte>();
            foreach (var brand in model.SelectedBrands)
            {
                // 2 ||| سامسونگ

                var splitBrand = brand.Split("|||");
                if (!byte.TryParse(splitBrand[1], out var commissionPercentage))
                {
                    return Json(new JsonResultOperation(false));
                }

                if (commissionPercentage > 20 || commissionPercentage < 1)
                {
                    return Json(new JsonResultOperation(false));
                }

                brandsInDictionary.Add(splitBrand[0], commissionPercentage);
            }
            //سامسونگ ||| 2 درصد
            // اینجا "سامسونگ" همون 
            //Key 
            //هست و ما در اصل فقط به همون سامسونگ نیار داریم نه درصدش
            //پس سلکت روی کییی میزنیم
            var brands = await _facadeServices.BrandService
                .GetBrandsByFullTitle(brandsInDictionary.Select(x => x.Key).ToList());
            // اگر کاربر سه برند را سمت کلاینت وارد کرد
            // باید همان مقدار را از پایگاه داده بخوانیم
            // و اگر اینطور نبود حتما یک یا چند برند را وارد کرده
            // که در پایگاه داده ما وجود ندارد
            if (model.SelectedBrands.Count != brands.Count)
            {
                return Json(new JsonResultOperation(false));
            }

            foreach (var brand in brands)
            {
                var commissionPercentage = brandsInDictionary[brand.Value];
                selectedCategory.CategoryBrands.Add(new CategoryBrand()
                {
                    BrandId = brand.Key,
                    CommissionPercentage = commissionPercentage
                    //در جدول 
                    //CateogryBrand
                    //CategoryId, Category, Brand
                    //هم داریم چرا اونا رو مپ نمکنیم پس؟
                    //CategoryId, Category
                    //رو مپ نمکنیم چون از طریق خود 
                    //Category
                    //اقدام میکنیم.
                });
            }
            //قبل اضافه رکردن درد کمیسیون
            // به این دلیل این کد پاک نشده که، روش فور ایچ زدن روی لیتی از آیدی ها
            // رو یاد بگیریم
            //var brandIds = await _facadeServices.BrandService.GetBrandIdsByList(model.SelectedBrands);
            //brandIds.ForEach(brandId => selectedCategory.CategoryBrands.Add(new CategoryBrand()
            //{
            //    BrandId = brandId,
                
                
            //}));
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "تغییرات با موفقیت ذخیره شد."));
        }

        public async Task<IActionResult> OnGetAutocompleteSearch(string term)
        {
            return Json(await _facadeServices.BrandService.AutocompleteSearch(term));
        }


        public async Task<IActionResult> OnGetEditCategoryVariant(long categoryId)
        {
            if (!await _facadeServices.CategoryService.IsExistsBy(nameof(MarketKhoone.Entities.Category.Id),
                    categoryId))
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            var isVariantTypeColor = await _facadeServices.CategoryService.IsVariantTypeColor(categoryId);

            if (isVariantTypeColor is null)
            {
                return Json(new JsonResultOperation(false));
            }

            var variants =
                await _facadeServices.VariantService.GetVariantsForEditCategoryVariants(isVariantTypeColor.Value);


            var selectedVariants =
                await _facadeServices.CategoryVariantService.GetCategoryVariants(categoryId);

            var model = new EditCategoryVariantViewModel()
            {
                //چون اسم پراپرتی و پارامتر ورودی یکی است
                // به صورت خودکار این رو به هم بایند میشن و نیازی به نوشتن نیست
                //CategoryId = categoryId
                Variants = variants,
                SelectedVariants = selectedVariants,
                
                //برای مثال این دسته بندی 3 رنگ دارد
                // از کدام یک از این رنگ ها در بخش تنوع محصولات استغاده شده است
                //آیدی اون تنوع ها رو برگشت میزنیم
                //که به ادمین اجازه ندیم که اون تنوع ها رو از این دسته بندی حذف کنه
                AddedVariantsToProductVariants = await _facadeServices.ProductVariantService
                    .GetAddedVariantToProductVariants(selectedVariants, categoryId)
            };
            return Partial("_EditCategoryVariantPartial", model);
        }

        public async Task<IActionResult> OnPostEditCategoryVariant(EditCategoryVariantViewModel model)
        {
            //جدول تنوع رو هم اینکلود میکنیم که تمامی رکورد هاش رو برگشت زده
            // که بتونیم همه شونو حذف کنیم و از نو برای این دسته بندی تنوع اضافه بکنیم
            var category = await _facadeServices.CategoryService.GetCategoryForEditVariant(model.CategoryId);

            if (category is null)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.RecordNotFoundMessage));
            }

            //دسته هایی که تنوع ندارند
            // کلا باید هیچ تنوعی بهشون اضافه بشه
            if (category.IsVariantColor is null)
            {
                return Json(new JsonResultOperation(false));
            }
            //لیست آیدی های تنوع های این دسته بندی
            var categoryVariantsIds = category.CategoryVariants.Select(x => x.VariantId).ToList();

            //آیا تنوع هایی که قراره برای این دسته بندی وارد شه
            //آیدیشون به درستی وارد شده
            //و اگر تنوع این دسته بندی رنگ باشد
            // باید توسط ادمین فقط رنگ به سمت سرور ارسال بشه

            if (!await _facadeServices.VariantService.CheckVariantsCountAndConfirmStatusForEditCategoryVariants(model.SelectedVariants,
                    category.IsVariantColor.Value))
            {
                return Json(new JsonResultOperation(false));
            }

            //برای مثال ان دسته بندی 3 رنگ دارد
            // از کدام یک از این 3 رنگ در بخش تنوع محصولات استفاده شده است
            //آیدی اون تنوع ها رو که در بخش تنوع محصولات استفاده شده است رو برگشت میزنیم
            var addedVariantsForProductVariants
                = await _facadeServices.ProductVariantService.GetAddedVariantToProductVariants(categoryVariantsIds,
                    model.CategoryId);

            // Category variants 10, 11, 13
            // Product Variants 10, 11

            foreach (var variant in category.CategoryVariants)
            {
                //برای مثال این دسته بندی رنگ آبی دارد
                // حالا در بخش تنوع محصولات هم از این رنگ آبی استفاده شده است
                // پس نباید اجازه دهیم ه رنگ آبی دیگر حذف شود
                // چون از رنگ آبی در بخش تنوع محصولات استفاده شده است
                if(addedVariantsForProductVariants.Contains(variant.VariantId))
                    continue;
                category.CategoryVariants.Remove(variant);
            }

            // تنوع ها را اضافه میکنیم
            foreach (var variantId in model.SelectedVariants)
            {
                if (category.CategoryVariants.Any(x => x.VariantId == variantId))
                    continue;

               

                category.CategoryVariants.Add(new CategoryVariant()
                {
                    VariantId = variantId
                });
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "تنوع های دسته بندی مئرد نظر با موفقیت اضافه شدند."));
        }
    }
}
