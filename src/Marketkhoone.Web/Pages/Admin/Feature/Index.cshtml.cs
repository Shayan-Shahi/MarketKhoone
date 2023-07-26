using AutoMapper;
using EShopMarket.Common.Helpers;
using MarketKhoone.Common;
using MarketKhoone.Common.Constants;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts.IFacadePattern;
using MarketKhoone.ViewModels.Features;
using Microsoft.AspNetCore.Mvc;

namespace Marketkhoone.Web.Pages.Admin.Feature
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



        public ShowFeaturesViewModel Features { get; set; } = new();
        public async Task OnGet()
        {
            var categories = await _facadeServices.CategoryService.GetCategoriesToShowInSelectBoxAsync();
            Features.SearchFeatures.Categories = categories.CreateSelectListItem();
        }

        public async Task<IActionResult> OnPostAddAsync(AddFeatureViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }

            var searchedTitle = model.Title.Trim();
            var feature = await _facadeServices.FeatureService.FindByTitleAsync(searchedTitle);

            if (feature is null)
            {
                await _facadeServices.FeatureService.AddAsync(new MarketKhoone.Entities.Feature()
                {
                    Title = searchedTitle,
                    CategoryFeatures = new List<CategoryFeature>()
                    {
                        new CategoryFeature()
                        {
                            CategoryId = model.CategoryId
                        }
                    }
                });
            }
            else
            {
                var categoryFeature = await _facadeServices.CategoryFeatureService
                    .GetCategoryFeature(model.CategoryId, feature.Id);
                if (categoryFeature is null)
                {
                    feature.CategoryFeatures.Add(new CategoryFeature()
                    {
                        CategoryId = model.CategoryId
                    });
                }
            }

            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "ویژگی های مورد نظر با موفقیت اضافه شد"));
        }

        public async Task<IActionResult> OnGetGetDataTableAsync(ShowFeaturesViewModel features)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResultOperation(false, PublicConstantStrings.ModelStateErrorMessage)
                {
                    Data = ModelState.GetModelStateErrors()
                });
            }
            return Partial("List", await _facadeServices.FeatureService.GetCategoryFeatures(features));
        }

        public async Task<IActionResult> OnPostDelete(long categoryId, long featureId)
        {
            var categoryFeature =
                await _facadeServices.CategoryFeatureService.GetCategoryFeature(categoryId, featureId);

            if (categoryFeature is null)
            {

                return Json(new JsonResultOperation(false, "ویژگی دسته بندی مورد نظر حذف نشد"));
            }

            _facadeServices.CategoryFeatureService.Remove(categoryFeature);
            await _uow.SaveChangesAsync();
            return Json(new JsonResultOperation(true, "ویژگی دسته بندی مورد نظر با موفقیت حذف شد"));
        }

        public async Task<IActionResult> OnGetAdd(long categoryId)
        {
            var categories = await _facadeServices.CategoryService.GetCategoriesWithNoChild();
            var model = new AddFeatureViewModel()
            {
                //CategoryId = categoryId
                Categories = categories.CreateSelectListItem()
            };

            return Partial("Add", model);
        }

        public async Task<IActionResult> OnGetAutoCompleteSearch(string term)
        {
            return Json(await _facadeServices.FeatureService.AutocompleteSearch(term));
        }
    }

}
