using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Brands;
using MarketKhoone.ViewModels.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class BrandService : GenericService<Brand>, IBrandService
    {

        #region Constructor
        private readonly DbSet<Brand> _brands;
        private readonly IMapper _mapper;

        public BrandService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _brands = uow.Set<Brand>();
        }
        #endregion


        public async Task<ShowBrandsViewModel> GetBrands(ShowBrandsViewModel model)
        {
            var brands = _brands.AsNoTracking().AsQueryable();
            #region Search
            brands = ExpressionHelpers.CreateSearchExpressions(brands, model.SearchBrands);
            #endregion

            #region OrderBy
            brands = brands.CreateOrderByExpression(model.SearchBrands.SortingBrands.ToString(),
                model.SearchBrands.SortingOrder.ToString());


            //از متد
            //Substring()
            //استفاده می کنیم. یعنی میگیم بیا و در یه کلمه مثلا 12 کاراکتری مثلا 4 کاراکتر رو درنظر نگیر و
            // مرتب سازی رو از کاراکتر شماره 5 انجام بده
            //http: 7 character
            //https: 8 character
            if (model.SearchBrands.SortingBrands == SortingBrands.BrandLinkEn)
            {
                if (model.SearchBrands.SortingOrder == SortingOrder.Asc)
                {
                    brands = brands.OrderBy(x => x.BrandLinkEn.Substring(
                        x.BrandLinkEn.StartsWith("https://") ? 8 : 7));
                }
                else
                {
                    brands = brands.OrderByDescending(x => x.BrandLinkEn.Substring(
                            x.BrandLinkEn.StartsWith("https://") ? 8 : 7));
                }
            }
            else if (model.SearchBrands.SortingBrands == SortingBrands.JudiciaryLink)
            {
                if (model.SearchBrands.SortingOrder == SortingOrder.Asc)
                {
                    brands = brands.OrderBy(x => x.BrandLinkEn.Substring(
                        x.JudiciaryLink.StartsWith("https://") ? 8 : 7));
                }
                else
                {
                    brands = brands.OrderByDescending(x => x.JudiciaryLink.Substring(
                        x.JudiciaryLink.StartsWith("https://") ? 8 : 7));
                }
            }


            #endregion

            var paginationResult = await GenericPaginationAsync(brands, model.Pagination);

            return new()
            {
                Brands = await _mapper.ProjectTo<ShowBrandViewModel>(
                    paginationResult.Query
                ).ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public Task<EditBrandViewModel> GetForEdit(long id)
        {
            return _mapper.ProjectTo<EditBrandViewModel>(
                _brands
            ).SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<string>> AutocompleteSearch(string term)
        {
            return await _brands
                .Where(x => x.TitleFa.Contains(term) || x.TitleEn.Contains(term))
                .OrderBy(x=>x.Id)
                .Take(20)
                .Select(x => x.TitleFa + " " + x.TitleEn)
                .ToListAsync();
        }

        public async Task<List<long>> GetBrandIdsByList(List<string> selectedBrands)
        {
            return await _brands
                .Where(x => selectedBrands.Contains(x.TitleFa + " " + x.TitleEn))
                .Select(x => x.Id)
                .ToListAsync();
        }

        public Task<Dictionary<long, string>> GetBrandsByCategoryId(long categoryId)
        {
            return _brands
                .SelectMany(x => x.CategoryBrands)
                .Where(x => x.CategoryId == categoryId)
                .Include(x => x.Brand)
                .ToDictionaryAsync(x => x.BrandId,
            x => x.Brand.TitleFa + " " + x.Brand.TitleEn);
        }

        public Task<BrandDetailsViewModel> GetBrandDetails(long brandId)
        {
            return _mapper.ProjectTo<BrandDetailsViewModel>(_brands.AsNoTracking())
                .SingleOrDefaultAsync(x => x.Id == brandId);
        }

        public Task<Brand> GetInActiveBrand(long id)
        {
            return _brands
                .Where(x => !x.IsConfirmed)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public Task<Dictionary<long, string>> GetBrandsByFullTitle(List<string> brandTitles)
        {
            return _brands.Where(x => brandTitles.Contains(x.TitleFa + " " + x.TitleEn))
                .ToDictionaryAsync(x => x.Id, x => x.TitleFa + " " + x.TitleEn);
        }

        public override async Task<DuplicateColumns> AddAsync(Brand entity)
        {
            var result = new List<string>();

            if (await _brands.AnyAsync(x => x.TitleFa == entity.TitleFa))
                result.Add(nameof(Brand.TitleFa));

            if (await _brands.AnyAsync(x => x.TitleEn == entity.TitleEn))
                result.Add(nameof(Brand.TitleEn));

            if (await _brands.AnyAsync(x => x.Slug == entity.Slug))
                result.Add(nameof(Brand.Slug));

            if (!result.Any())
                await base.AddAsync(entity);
            return new(!result.Any())
            {
                Columns = result
            };
        }

        public override async Task<DuplicateColumns> Update(Brand entity)
        {
            var query = _brands.Where(x => x.Id != entity.Id);
            var result = new List<string>();

            if (await query.AnyAsync(x => x.TitleFa == entity.TitleFa))
                result.Add(nameof(Brand.TitleFa));

            if (await query.AnyAsync(x => x.TitleEn == entity.TitleEn))
                result.Add(nameof(Brand.TitleEn));

            if (await query.AnyAsync(x => x.Slug == entity.Slug))
                result.Add(nameof(Brand.Slug));

            if (!result.Any())
                await base.Update(entity);
            return new(!result.Any())
            {
                Columns = result
            };
        }
    }
}
