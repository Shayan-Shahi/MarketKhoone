﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketKhoone.Common.Helpers;
using MarketKhoone.Common.IdentityToolkit;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Categories;
using MarketKhoone.ViewModels.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class CategoryService : GenericService<Category>, ICategoryService
    {

        #region Constructor
        private readonly DbSet<Category> _categories;
        private readonly DbSet<Product> _products;
        private readonly DbSet<Seller> _sellers;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContexAccessor) : base(uow)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContexAccessor;
            _categories = uow.Set<Category>();
            _products = uow.Set<Product>();
            _sellers = uow.Set<Seller>();
        }
        #endregion

        public async Task<ShowCategoriesViewModel> GetCategories(ShowCategoriesViewModel model)
        {

            var categories = _categories.AsNoTracking().AsQueryable();

            #region Search
            categories = ExpressionHelpers.CreateSearchExpressions(categories, model.SearchCategories);
            #endregion

            #region OrderBy
            categories = categories.CreateOrderByExpression(model.SearchCategories.SortingCategories.ToString(),
                model.SearchCategories.SortingOrder.ToString());
            #endregion

            #region Pagination
            var paginationResult = await GenericPaginationAsync(categories, model.Pagination);
            #endregion

            return new()
            {
                Categories = await _mapper.ProjectTo<ShowCategoryViewModel>(paginationResult.Query).ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public async Task<Dictionary<long, string>> GetCategoriesToShowInSelectBoxAsync(long? id = null)
        {
            return await _categories.AsNoTracking()
                .Where(x => id == null || x.Id != id)
                .Take(70)
                .ToDictionaryAsync(x => x.Id, x => x.Title);
            //return await _categories.AsNoTracking()
            //    .Where(x => x.ParentId != null)
            //    .Take(20)
            //    .ToDictionaryAsync(x => x.Id, x => x.Title);


        }

        public async Task<EditCategoryViewModel> GetForEdit(long id)
        {

            return await _mapper.ProjectTo<EditCategoryViewModel>(_categories)
                .SingleOrDefaultAsync(x => x.Id == id);
            //return await _categories.Select(x => new EditCategoryViewModel()
            //{
            //    SelectedPicture = x.Picture,
            //    ParentId = x.ParentId,
            //    Id = x.Id,
            //    Description = x.Description,
            //    Title = x.Title,
            //    Slug = x.Slug,
            //    ShowInMenus = x.ShowInMenus
            //}).SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<List<ShowCategoryForCreateProductViewModel>>> GetCategoriesForCreateProduct(long[] selectedCategoriesIds)
        {
            var result = new List<List<ShowCategoryForCreateProductViewModel>>
            {
                await _categories.Where(x => x.ParentId == null).Where(x=>!x.IsDeleted)
                    .Select(x => new ShowCategoryForCreateProductViewModel()
                    {
                        Title = x.Title,
                        HasChild = x.Categories.Any(),
                        Id = x.Id
                    }).ToListAsync()
            };
            for (var counter = 0; counter < selectedCategoriesIds.Length; counter++)
            {
                var selectedCategoryId = selectedCategoriesIds[counter];
                result.Add(
                    await _categories.Where(x => x.ParentId == selectedCategoryId)
                        .Select(x => new ShowCategoryForCreateProductViewModel()
                        {
                            Title = x.Title,
                            HasChild = x.Categories.Any(),
                            Id = x.Id
                        }).ToListAsync()
                );
            }

            return result;
        }

        public Task<List<string>> GetCategoryBrands(long selectedCategoryId)
        {
            return _categories
                .Where(x => x.Id == selectedCategoryId)
                .SelectMany(x => x.CategoryBrands)
                .Select(x => x.Brand.TitleFa + " " + x.Brand.TitleEn + "|||" + x.CommissionPercentage)
                .ToListAsync();
        }

        public Task<Category> GetCategoryWithItsBrands(long selectedCategoryId)
        {
            return _categories
                .Include(x => x.CategoryBrands)
                .SingleOrDefaultAsync(x => x.Id == selectedCategoryId);
        }

        public async Task<bool> CanAddFakeProduct(long categoryId)
        {
            //var category = await _categories.Select(x => new
            //{
            //    x.Id,
            //    x.CanAddFakeProduct
            //}).SingleOrDefaultAsync(x => x.Id == categoryId);
            //return category?.CanAddFakeProduct ?? false;

            var category = await _categories
                .Where(x => x.Id == categoryId)
                .Select(x => new
            {
                x.CanAddFakeProduct
            }).SingleOrDefaultAsync();
            return category?.CanAddFakeProduct ?? false;
        }

        //دسته بندی این محصول کدومه
        // کالای دیجیتال=>گوشی موبایل=>اس هفت
        // همون گوشی موبایل رو برگشت میزنه 
        public Task<Dictionary<long, string>> GetCategoriesWithNoChild()
        {
            return _categories.Where(x => !x.Categories.Any())
                .ToDictionaryAsync(x => x.Id, x => x.Title);
        }

        public Task<Dictionary<long, string>> GetLastCategoriesWithNoChild()
        {
            return _categories
                .Where(x => !x.Categories.Any())
                //.Where(x => x.ParentId !=x.Id)
                .Where(x=>x.ParentId != null)
                .ToDictionaryAsync(x => x.Id, x => x.Title);
        }

        public async Task<bool> CheckProductCategoryIdsInComparePage(params int[] productCodes)
        {
            var mainCategoryIds = await _products.Where(x => productCodes.Contains(x.ProductCode))
                .Select(x => x.MainCategoryId).ToListAsync();

            if (mainCategoryIds.Count < 1)
            {
                return false;
            }
            //آیا تمامی دسته بندی های محصولات داخل لیست مشابه هم هستند یا نه؟
            return mainCategoryIds.Distinct().Count() == 1;
        }

        public async Task<Dictionary<long, string>> GetSellerCategories()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetLoggedInUserId();
            var sellerId = await _sellers
                .Where(x => x.UserId == userId)
                .Select(x => x.Id).SingleOrDefaultAsync();

            return await _products.Where(x => x.SellerId == sellerId)
                //چرا گروپ باین؟
                // چون ممکنه 100 تا محصول با دسته بندی کوشی موبایل داشته باشه
                // گوشی موبایل  یه دسته بندی هست، پس ممکنه 100 محصول یا یه آیدی یکسان برای دسته بندی اش برگشت بزنیم
                // پس گروپ بای میکنیم که تکراری ها حذف بشن
                .GroupBy(x => x.MainCategoryId)
                .Select(x => new
                {
                    x.Key,
                    x.First().Category.Title

                }).ToDictionaryAsync(x => x.Key, x => x.Title);
        }

        public async Task<(bool isSuccessful, List<long> categoryIds)> GetCategoryParentIds(long categoryId)
        {
            if (!await IsExistsBy(nameof(MarketKhoone.Entities.Category.Id), categoryId))
            {
                return (false, new List<long>());
            }

            if (await _categories.AnyAsync(x => x.ParentId == categoryId))
            {
                return (false, new List<long>());
            }
            //// کالای دیجیتال=>گوشی موبایل=>اس هفت
            //داحل کروشه اون 
            //categoryId
            //چیه؟ این همون آیدی گوشی موبایله که باید اون رو هم به لیست اضافه کنیم
            var result = new List<long>() { categoryId };
            var currentCategoryId = categoryId;

            //اونقدر بگرد تا یه دسته بندی با پرنت آیدی نال پیدا بشه
            while (true)
            {
                var categoryToAdd = await _categories
                    .Select(x => new
                    {
                        x.Id,
                        x.ParentId
                    })
                    .SingleOrDefaultAsync(x => x.Id == currentCategoryId);
                //رسیدیم به کالای دیجیتال
                if (categoryToAdd.ParentId is null)
                {
                    break;
                }

                currentCategoryId = categoryToAdd.ParentId.Value;
                result.Add(categoryToAdd.ParentId.Value);
            }

            return (true, result);
        }

        public Task<SearchOnCategoryViewModel> GetSearchOnCategoryData(string categorySlug, string brandSlug)
        {
            //return _mapper.ProjectTo<SearchOnCategoryViewModel>(_categories.Where(x => x.Slug == categorySlug)
            //        .SelectMany(x => x.CategoryBrands)
            //        .Select(x => x.Brand.Slug == brandSlug))
            //    .SingleOrDefaultAsync();

            return _categories
                .AsNoTracking().AsSplitQuery()
                .Where(x => x.Slug == categorySlug)
                .ProjectTo<SearchOnCategoryViewModel>(configuration: _mapper.ConfigurationProvider,
                    parameters: new { brandSlug }).SingleOrDefaultAsync();
        }

        public Task<List<string>> GetCategoryVariants(long selectedCategoryId)
        {
            return _categories
                .Where(x => x.Id == selectedCategoryId)
                .SelectMany(x => x.CategoryVariants)
                .Select(x => x.Variant.Value)
                .ToListAsync();
        }

        public async Task<bool?> IsVariantTypeColor(long categoryId)
        {
            return await _categories.Where(x => x.Id == categoryId)
                .Select(x=>x.IsVariantColor)
                .SingleOrDefaultAsync();
        }

        public Task<Category> GetCategoryForEditVariant(long categoryId)
        {
            return _categories
                .Where(x => x.Id == categoryId)
                .Include(x=>x.CategoryVariants)
                .SingleOrDefaultAsync();
        }

        public override async Task<DuplicateColumns> AddAsync(Category entity)
        {
            var result = new List<string>();
            if (await _categories.AnyAsync(x => x.Title == entity.Title))
                result.Add(nameof(Category.Title));

            if (await _categories.AnyAsync(x => x.Slug == entity.Slug))
                result.Add(nameof(Category.Slug));
            if (!result.Any())
                await base.AddAsync(entity);
            return new(!result.Any())
            {
                Columns = result
            };
        }
        public override async Task<DuplicateColumns> Update(Category entity)
        {
            var query = _categories.Where(x => x.Id != entity.Id);
            var result = new List<string>();
            if (await _categories.AnyAsync(x => x.Title == entity.Title))
                result.Add(nameof(Category.Title));

            if (await _categories.AnyAsync(x => x.Slug == entity.Slug))
                result.Add(nameof(Category.Slug));
            if (!result.Any())
                await base.Update(entity);
            return new(!result.Any())
            {
                Columns = result
            };
        }

    }
}
