using AutoMapper;
using EShopMarket.Common.Helpers;
using Ganss.Xss;
using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.Services.Services;
using MarketKhoone.ViewModels.Brands;
using MarketKhoone.ViewModels.CategoryFeatures;
using MarketKhoone.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Marketkhoone.Web.Pages.SellerPanel.Product
{
    public class CreateModel : PageBase
    {
        #region Constructor

        private readonly IFacadeServices _facadeServices;
        private readonly IMapper _mapper;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IUnitOfWork _uow;
        private readonly IViewRendererService _viewRendererService;

        public CreateModel(IFacadeServices facadeServices,
            IMapper mapper, IHtmlSanitizer htmlSanitizer, IViewRendererService viewRendererService, IUnitOfWork uow)
        {
            _facadeServices = facadeServices;
            _mapper = mapper;
            _htmlSanitizer = htmlSanitizer;
            _viewRendererService = viewRendererService;
            _uow = uow;
        }

        #endregion

        public AddProductViewModel Product { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(AddProductViewModel product)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            var categoriesToAdd = await _facadeServices.CategoryService.GetCategoryParentIds(product.MainCategoryId);
            if (!categoriesToAdd.isSuccessful)
            {
                return Json(new JsonResultOperation(false));
            }
            //آیا برند وارد شده برای همین دسته بندی است یا نه-- تب دوم، وقتی کاربر داره از سلکت باکس برای
            //دسته بندی ایی که در تب اول، انتخاب کرده، برند انتخاب میکنه، برای حالتی که ممکنه مقدار سلکت باکس رو
            //با ایسنپکت تغییر بده
            //این برند سامسونگ اصلا برای دسته بندی گوشی موبایل هست در دیتابیس  
            //شاید در دسته بندی گوشی موبایل برندی بنام سامسونگ نداشته باشیم
            if (!await _facadeServices.CategoryBrandService.CheckCategoryBrand(product.MainCategoryId, product.BrandId))
            {
                return Json(new JsonResultOperation(false));
            }

            var productToAdd = _mapper.Map<MarketKhoone.Entities.Product>(product);


            var shortLink = await _facadeServices.ProductShortLinkService.GetProductShortLinkForCreateProduct();
            productToAdd.ProductShortLinkId = shortLink.Id;
            shortLink.IsUsed = true;

            productToAdd.Slug = productToAdd.PersianTitle.ToUrlSlug();
            var userId = User.Identity.GetLoggedInUserId();
            productToAdd.SellerId = await _facadeServices.SellerService.GetSellerId(userId);
            productToAdd.ProductCode = await _facadeServices.ProductService.GetProductCodeForCreateProduct();

            if (string.IsNullOrWhiteSpace(product.ShortDescription))
            {
                productToAdd.ShortDescription = null;
            }
            else
            {
                productToAdd.ShortDescription = _htmlSanitizer.Sanitize(product.ShortDescription);
            }

            if (string.IsNullOrWhiteSpace(product.SpecialtyCheck))
            {
                productToAdd.SpecialtyCheck = null;
            }
            else
            {
                productToAdd.SpecialtyCheck = _htmlSanitizer.Sanitize(productToAdd.SpecialtyCheck);
            }
            //کاربر با بازی کردن با اینسپکت میتونه
            //دسته بندی که نمیشه بهش محصول عیر اصل ایجاد کرد رو ترو کنه مقدار چکباکس رو
            //و سمت سرور بفرسته، پس ما باید چک کنیم، آیا برای دسته بندی اومده میشه کالای اصل/غیراصل اضافه 
            //کرد یا نه---تب دوم، اون چکباکسه
            if (!await _facadeServices.CategoryService.CanAddFakeProduct(product.MainCategoryId))
            {
                productToAdd.IsFake = false;
            }

            foreach (var categoryId in categoriesToAdd.categoryIds)
            {
                //پس باید در نویگیشن پروداکت ان پروداکت کتگوری را ، نمونه سازی کنیم چون داریم اد میکنیم
                productToAdd.ProductCategories.Add(new ProductCategory()
                {
                    //چون از ظریق پروداکت اثدام میکنیم
                    // پروداکت آیدی رو نمیخواد بایند کنیم

                    //ProductId = productToAdd.Id,
                    CategoryId = categoryId

                });
            }

            foreach (var picture in product.Pictures)
            {
                if (picture.IsFileUploaded())
                {
                    var fileName = picture.GenerateFileName();
                    // پس باید نویگیشنه پروداکت مدیا رو در پروداکت اد کنیم
                    productToAdd.ProductMedia.Add(new ProductMedia()
                    {
                        FileName = fileName,
                        IsVideo = false,
                        //از ط ریق پروداکت اقدام میکنیم، پس مقدار دهی
                        // پروداکت آیدی لازم نیست
                    });
                }
            }

            foreach (var video in product.Videos)
            {
                if (video.IsFileUploaded())
                {
                    var fileName = video.GenerateFileName();
                    productToAdd.ProductMedia.Add(new ProductMedia()
                    {
                        FileName = fileName,
                        IsVideo = true
                    });
                }
            }
            #region NonConstantValue

            var featureIds = new List<long>();
            var productFeatureValueInputs = Request.Form
                .Where(x => x.Key.StartsWith("ProductFeatureValue")).ToList();

            foreach (var item in productFeatureValueInputs)
            {
                if (long.TryParse(item.Key.Replace("ProductFeatureValue", string.Empty), out var featureId))
                {
                    featureIds.Add(featureId);
                }
                else
                {
                    return Json(new JsonResultOperation(false));
                }
            }
            //مقادیر غیر ثابت مبادا در جدول ثابت ها باشد
            //مثلا مقدار رم که کاربر خودش میگه 4 گیگه، 6 گیگه یا 8 گیگه
            // با فیچر تعداد دوربین که ما خودمون در سلکت باکس به کارر گفتیم ، یک یا دو یا سه عدد
            // و کاربر میاد و انتخاب میکنه از بین این یک و دو سه، در یه جدول قاطی بشن
            // مقدار فیچر که میاد،  چک میکنیم اون فیچر آیدی متعلق به همون جدول خودش-قابت یا غیر قابت باشه حتما
            if (await _facadeServices.FeatureConstantValueService.CheckNonConstantValue(product.MainCategoryId,
                    featureIds))
            {
                return Json(new JsonResultOperation(false));
            }

            foreach (var item in productFeatureValueInputs)
            {
                if (long.TryParse(item.Key.Replace("ProductFeatureValue", string.Empty), out var featureId))
                {
                    var trimmedValue = item.Value.ToString().Trim();
                    //نمونه سازی از نویگیشن پروداکت فیچرز
                    //اگر همچین فیچری وجود نداشت اضاقه کن
                    //یعنی اگر مقدار رم اضافه شده بود، دیگه مقدار رم رو اضاقه نکن
                    // در ضمن در انتیتی 
                    //ProductFeature
                    //Index
                    //هم برای 
                    //ProductId, FeatureId
                    //به صورت همزمان یونیک کردیم.
                    if (productToAdd.ProductFeatures.All(x => x.FeatureId != featureId))
                    {
                        if (trimmedValue.Length > 0)
                        {
                            productToAdd.ProductFeatures.Add(new ProductFeature()
                            {
                                FeatureId = featureId,
                                Value = trimmedValue
                            });
                        }
                    }
                }
                else
                {
                    return Json(new JsonResultOperation(false));
                }
            }

            #endregion


            #region ConstantValue

            var featureConstantValueIds = new List<long>();

            var productFeatureConstantValueInputs =
                Request.Form.Where(x => x.Key.StartsWith("ProductFeatureConstantValue")).ToList();

            foreach (var item in productFeatureConstantValueInputs)
            {
                if (long.TryParse(item.Key.Replace("ProductFeatureConstantValue", string.Empty), out var featureId))
                {
                    featureConstantValueIds.Add(featureId);
                }
                else
                {
                    return Json(new JsonResultOperation(false));
                }

            }
            //ما نمیخواهیم بیاییم یه بار برای ویژگی های ثابت و یه بار هم برای ویژگی های غیر ثابت این چک(کامنت پایینی) رو انجام بدیم، 
            //پس لیست ویژگی ها یثابت با غیرثابت رو ترکیب میکنیم با کانکت تا یه بار به سمت سرور بریم

            //فیچر آیدی یه لیست، کانکت یه 
            //IEnumerable
            //برگشت میزنه پس دات تو لیست میکنیم
            featureIds = featureIds.Concat(featureConstantValueIds).ToList();

            //آیا این آیدی از دسته بندی، کلا اون فیچر آیدی رو که کاربر مقدارش رو براب ما فرستاده رو دارن یا نه
            //خودمون به کاربر نشون دادیم ولی نکنه کاربر با اینسپکت اونا رو عوض کنه- ما اینجا نگران مقدار شون نیستیم ها
            // اصلا همون هایی که ما فرستادیم رو داره برگشت میزنه یا نه

            if (!await _facadeServices.CategoryFeatureService.CheckCategoryFeatureCount(product.MainCategoryId, featureIds))
            {
                return Json(new JsonResultOperation(false));
            }

            if (!await _facadeServices.FeatureConstantValueService.CheckConstantValue(product.MainCategoryId,
                    featureConstantValueIds))
            {
                return Json(new JsonResultOperation(false));
            }

            //مقدار مقادیر ثابتی که کاربر برای یه دسته بندی فرستاده رو خودمون میریم
            // از دیتابیس میاریم، ببینیم اصلا این دسته بندی ما، این مقادیری از فیچر(مقادیر ثابت ما) که کاربر فرستاده رو داره
            // یا کاربر با اینیسپکت شلوغ کاری کرده
            var featureConstantValues =
                await _facadeServices.FeatureConstantValueService.GetFeatureConstantValuesForCreateProduct(
                    product.MainCategoryId);

            foreach (var item in productFeatureConstantValueInputs)
            {
                if (long.TryParse(item.Key.Replace("ProductFeatureConstantValue", string.Empty), out var featureId))
                {
                    if (item.Value.Count > 0)
                    {
                        //string
                        //داخل فور ایچ بهینه نیست از 
                        //StringBuilder
                        //استفاده میکنیم
                        var valueToAdd = new StringBuilder();
                        foreach (var value in item.Value)
                        {
                            var trimmedValue = value.Trim();

                            //آیا این فیچر کانستن ولیووی که ما خودمون از دیتابیس گرفتیم
                            // شامل این فیچرآی دی با این مقدار ولییوی هست که کاربر فرستاده
                            if (featureConstantValues.Where(x => x.FeatureId == featureId)
                                .Any(x => x.Value == trimmedValue))
                            {
                                valueToAdd.Append(trimmedValue + "|||");
                            }
                        }

                        if (productToAdd.ProductFeatures.All(x => x.FeatureId != featureId))
                        {
                            if (valueToAdd.ToString().Length > 0)
                            {
                                //a 
                                //همون b هستش
                                var a = valueToAdd.ToString()[..(valueToAdd.Length - 3)];
                                var b = valueToAdd.ToString().Substring(0, valueToAdd.Length - 3);
                                productToAdd.ProductFeatures.Add(new()
                                {
                                    FeatureId = featureId,
                                    Value = valueToAdd.ToString()[..(valueToAdd.Length - 3)]
                                });
                            }
                        }
                    }
                }
                else
                {
                    return Json(new JsonResultOperation(false));
                }
            }
            #endregion

            await _facadeServices.ProductService.AddAsync(productToAdd);
            await _uow.SaveChangesAsync();


            var productPictures = productToAdd.ProductMedia
                .Where(x => !x.IsVideo)
                .ToList();

            for (int counter = 0; counter < productPictures.Count; counter++)
            {
                var currentPicture = product.Pictures[counter];
                if (currentPicture.IsFileUploaded())
                {
                    await _facadeServices.UploadedFileService.SaveFile(currentPicture,
                        productPictures[counter].FileName, null, "images", "products");
                }
            }

            var productVideos = productToAdd.ProductMedia
                .Where(x => x.IsVideo)
                .ToList();

            for (int counter = 0; counter < productVideos.Count; counter++)
            {
                var currentVideo = product.Videos[counter];
                if (currentVideo.IsFileUploaded())
                {
                    await _facadeServices.UploadedFileService.SaveFile(currentVideo, productVideos[counter].FileName,
                        null,
                        "videos", "products");
                }
            }

            return Json(new JsonResultOperation(true, "محصول مورد نظر با موفقیت ایجاد شد")
            {
                Data = Url.Page(nameof(Successful))
            });

        }

        public void Successful()
        {

        }

        public async Task<IActionResult> OnGetGetCategories(long[] selectedCategoriesIds)
        {
            var result =
                await _facadeServices.CategoryService.GetCategoriesForCreateProduct(selectedCategoriesIds);
            return Partial("_SelectProductCategoryPartial", result);
        }

        public async Task<IActionResult> OnGetGetCategoryInfo(long categoryId)
        {
            if (categoryId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var categoryFeatureModel = new ProductFeaturesForCreateProductViewModel()
            {
                Features = await _facadeServices.CategoryFeatureService.GetCategoryFeatures(categoryId),
                FeaturesConstantValues =
                    await _facadeServices.FeatureConstantValueService.GetFeatureConstantValuesByCategoryId(categoryId),
            };

            var model = new
            {
                Brands = await _facadeServices.BrandService.GetBrandsByCategoryId(categoryId),
                CanAddFakeProduct = await _facadeServices.CategoryService.CanAddFakeProduct(categoryId),
                CategoryFeatures = await _viewRendererService.RenderViewToStringAsync
                    ("~/Pages/SellerPanel/Product/_ShowCategoryFeaturesPartial.cshtml", categoryFeatureModel)
            };
            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = model
            });
        }

        public IActionResult OnGetRequestForAddBrand(long categoryId)
        {
            //چون دو تا 
            //CategoryId 
            //ها همنام هستن، پس لازم نیست کد پایین رو بنویسییم
           // var model = new AddBrandBySellerViewModel() { CategoryId = categoryId };
            return Partial("_RequestForAddBrandPartial");
        }

        public async Task<IActionResult> OnPostRequestForAddBrand(AddBrandBySellerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            var brand = _mapper.Map<MarketKhoone.Entities.Brand>(model);

            // Add brand category

            brand.CategoryBrands.Add(new CategoryBrand()
            {
                //brand, brandId
                // رو هم نمیخواد پر کنیم چون ما از طریق خود برند اقدام کرده ایم
                CategoryId = model.CategoryId
            });

            var userId = User.Identity.GetUserId();
            brand.SellerId = await _facadeServices.SellerService.GetSellerId(userId.Value);

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
            await _facadeServices.UploadedFileService.SaveFile(model.LogoPicture, brand.LogoPicture, null, "images",
                "brands");
            await _facadeServices.UploadedFileService.SaveFile(model.BrandRegistrationPicture,
                brandRegistrationFileName, null, "images", "brandregistrationpictures");
            return Json(new JsonResultOperation(true,
                "برند ثبت شد و پس از تایید، مراتب از طریق ایمیل اظلاع رسانی خواهد شد"));
        }

        public async Task<IActionResult> OnGetCheckForTitleFa(string titleFa)
        {
            return Json(
                !await _facadeServices.BrandService.IsExistsBy(nameof(MarketKhoone.Entities.Brand.TitleFa), titleFa));
        }

        public async Task<IActionResult> OnGetCheckForTitleEn(string titleEn)
        {
            return Json(
                !await _facadeServices.BrandService.IsExistsBy(nameof(MarketKhoone.Entities.Brand.TitleEn), titleEn));
        }

        //با اضافه کردن 
        //[IsImage]
        //دیگه نیاز به ایجاد ویوو مدل نداریم.
        public IActionResult OnPostUploadSpecialtyCheckImage([IsImage] IFormFile file)
        {
            if (ModelState.IsValid && file.IsFileUploaded())
            {
                var imageFileName = file.GenerateFileName();
                _facadeServices.UploadedFileService.SaveFile(file, imageFileName, null,
                    "images", "products", "specialty-check-images");
                return Json(new
                {
                    location = $"/images/products/specialty-check-images/{imageFileName}"
                });
            }

            return Json(false);
        }

        public IActionResult OnPostUploadShortDescriptionImages([IsImage] IFormFile file)
        {
            if (ModelState.IsValid && file.IsFileUploaded())
            {
                var imageFileName = file.GenerateFileName();
                _facadeServices.UploadedFileService.SaveFile(file, "images", "products", "short-description-images");

                return Json(new
                {
                    location = $"/images/products/short-description-images/{imageFileName}"
                });
            }

            return Json(false);
        }

        public async Task<IActionResult> OnGetGetCommissionPercentage(long brandId, long categoryId)
        {
            if (brandId < 1 || categoryId < 1)
            {
                return Json(new JsonResultOperation(false));
            }

            var result = await _facadeServices.CategoryBrandService.GetCommissionPercentage(categoryId, brandId);
            if (!result.isSuccessful)
            {
                return Json(new JsonResultOperation(false));
            }

            return Json(new JsonResultOperation(true, string.Empty)
            {
                Data = result.value
            });
        }

    }
}

