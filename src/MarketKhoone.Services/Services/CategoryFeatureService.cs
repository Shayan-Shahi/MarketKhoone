  using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.CategoryFeatures;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class CategoryFeatureService : CustomGenericService<CategoryFeature>, ICategoryFeatureService
    {

        #region Constructor
        private readonly DbSet<CategoryFeature> _categoryFeatures;
        private readonly IMapper _mapper;

        public CategoryFeatureService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _categoryFeatures = uow.Set<CategoryFeature>();
        }
        #endregion



        public async Task<CategoryFeature> GetCategoryFeature(long categoryId, long featureId)
        {
            return await _categoryFeatures.Where(x => x.CategoryId == categoryId)
                .SingleOrDefaultAsync(x => x.FeatureId == featureId);
        }

        public async Task<List<CategoryFeatureForCreateProductViewModel>> GetCategoryFeatures(long categoryId)
        {
            return await _mapper.ProjectTo<CategoryFeatureForCreateProductViewModel>(_categoryFeatures
                .Where(x => x.CategoryId == categoryId)).ToListAsync();

        }

        public Task<bool> CheckCategoryFeature(long categoryId, long featureId)
        {
            return _categoryFeatures.Where(x => x.CategoryId == categoryId)
                .AnyAsync(x => x.FeatureId == featureId);
        }

        public async Task<bool> CheckCategoryFeatureCount(long categoryId, List<long> featureIds)
        {
            var featuresCount = await _categoryFeatures.Where(x => x.CategoryId == categoryId)
                .CountAsync(x => featureIds.Contains(x.FeatureId));

            return featuresCount == featureIds.Count();
        }
    }
}
