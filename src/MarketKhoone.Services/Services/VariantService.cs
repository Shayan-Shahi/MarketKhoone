using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Variants;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class VariantService : GenericService<Variant>, IVariantService
    {
        #region Constructor
        private readonly DbSet<Variant> _variants;
        private readonly DbSet<Product> _products;
        private readonly IMapper _mapper;
        public VariantService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _variants = uow.Set<Variant>();
            _products = uow.Set<Product>();
        }
        #endregion

        public async Task<ShowVariantsViewModel> GetVariants(ShowVariantsViewModel model)
        {
            var variants = _variants.AsNoTracking().AsQueryable();

            #region Search
            //چرا
            //callDeletedStatusExpression =false
            //هسست؟ چون در 
            //SearchVariantsViewModel
            // ما پراپرتی 
            //DeletedStatus 
            //را نداریم.
            variants = ExpressionHelpers.CreateSearchExpressions(variants, model.SearchVariants, callDeletedStatusExpression: false);

            #endregion

            #region OrderBy

            variants = variants.CreateOrderByExpression(model.SearchVariants.Sorting.ToString(),
                model.SearchVariants.SortingOrder.ToString());

            #endregion

            var paginationResult = await GenericPaginationAsync(variants, model.Pagination);

            return new()
            {
                Variants = await _mapper.ProjectTo<ShowVariantViewModel>(
                    paginationResult.Query
                ).ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public async Task<(bool isISuccessful, bool IsVariantNull)> CheckProductAndVariantTypeForAddVariant(long productId,
            long variantId)
        {

            var product = await _products
                .Select(x => new
                {
                    x.Id,
                    x.Category.IsVariantColor
                }).SingleOrDefaultAsync(x => x.Id == productId);


            if (product is null)
                return (false, default);

            if (product.IsVariantColor is null)
                return (false, true);

            var variant = await _variants
                .Where(x => x.IsConfirmed)
                .Select(x => new
                {
                    x.Id,
                    x.IsColor
                }).SingleOrDefaultAsync(x => x.Id == variantId);

            if (variant is null)
                return (false, false);


            return (product.IsVariantColor == variant.IsColor, false);

        }

        public async Task<List<ShowVariantInEditCategoryVariantViewModel>> GetVariantsForEditCategoryVariants(bool isVariantTypeColor)
        {
            return await _mapper.ProjectTo<ShowVariantInEditCategoryVariantViewModel>(
                _variants.Where(x => x.IsConfirmed)
                    .Where(x => x.IsColor == isVariantTypeColor)
                )
                .ToListAsync();
        }

        public async Task<bool> CheckVariantsCountAndConfirmStatusForEditCategoryVariants(List<long> categoryVariantsIds,
            bool categoryIsVariantColor)
        {
            var result= await _variants.Where(x => categoryVariantsIds.Contains(x.Id))
                .Where(x => x.IsColor == categoryIsVariantColor)
                .CountAsync(x => x.IsConfirmed);
            return categoryVariantsIds.Count == result;
        }


        public override async Task<DuplicateColumns> AddAsync(Variant entity)
        {
            var result = new List<string>();

            if (await _variants.AnyAsync(x => x.Value == entity.Value &&  x.ColorCode == entity.ColorCode))
                result.Add(nameof(Variant.Value));

            //if (await _variants.AnyAsync(x => x.ColorCode == entity.ColorCode))
            //    result.Add(nameof(Variant.ColorCode));


            if (!result.Any())
                await base.AddAsync(entity);
            return new(!result.Any())
            {
                Columns = result
            };
        }

        public override async Task<DuplicateColumns> Update(Variant entity)
        {
            var query = _variants.Where(x => x.Id != entity.Id);
            var result = new List<string>();

            if (await query.AnyAsync(x => x.Value == entity.Value))
                result.Add(nameof(Variant.Value));

            if (await query.AnyAsync(x => x.ColorCode == entity.ColorCode))
                result.Add(nameof(Variant.ColorCode));

            if (!result.Any())
                await base.Update(entity);

            return new(!result.Any())
            {
                Columns = result
            };
        }
    }
}
