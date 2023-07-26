using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class CategoryBrandService : CustomGenericService<CategoryBrand>, ICategoryBrandService
    {
        #region Constructor

        private readonly DbSet<CategoryBrand> _categoryBrands;
        private readonly IMapper _mapper;
        public CategoryBrandService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _categoryBrands = uow.Set<CategoryBrand>();
        }
        #endregion

        public async Task<bool> CheckCategoryBrand(long categoryId, long brandId)
        {
            return await _categoryBrands.Where(x => x.CategoryId == categoryId)
                .AnyAsync(x => x.BrandId == brandId);
        }

        public async Task<(bool isSuccessful, byte value)> GetCommissionPercentage(long categoryId, long brandId)
        {
            var query = await _categoryBrands.Where(x => x.CategoryId == categoryId)
                .Where(x => x.BrandId == brandId)
                .SingleOrDefaultAsync();

            return (query != null, query?.CommissionPercentage ?? 0);
        }
    }
}
