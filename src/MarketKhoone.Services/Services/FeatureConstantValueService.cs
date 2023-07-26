using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.FeatureConstantValues;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class FeatureConstantValueService : GenericService<FeatureConstantValue>, IFeatureConstantValueService
    {
        private readonly DbSet<FeatureConstantValue> _featureConstantValues;
        private readonly IMapper _mapper;
        public FeatureConstantValueService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _featureConstantValues = uow.Set<FeatureConstantValue>();
        }

        public async Task<ShowFeatureConstantValuesViewModel> GetFeatureConstantValues(ShowFeatureConstantValuesViewModel model)
        {
            var featureConstantValues = _featureConstantValues.AsNoTracking().AsQueryable();

            #region Search

            //چرا این سرج رو دستی نوشتیم؟
            // در سلکت باکسی که ویژگی ها رو براساس دسته بندی نشون میداد
            //اومدیم و مقدار 
           // <option value="0"></option>
           //دادیم، اگه بالای سر ویو مدل فیچر آیدی بیاییم و اتریبیوت
           //[EqualSearch]
           //رو قرار بدیم، موقعی که سلکت باکسش در حال انتخاب کنید است
           // فیچر آیدی صفر رو به سمت سرور برای سرچ میفرسته
           // پس اون اتریبوت رو باید از بالای فیچر آیدی برداریم
           // حالا که برداشتیم باید سرچش رو دستی بنویسیم
            var searchedFeatureId = model.SearchFeatureConstantValues.FeatureId;
            if (searchedFeatureId != 0)
            {
                featureConstantValues = featureConstantValues.Where(x => x.FeatureId == searchedFeatureId);
            }

            featureConstantValues = ExpressionHelpers.CreateSearchExpressions(featureConstantValues, model.SearchFeatureConstantValues);

            #endregion

            #region OrderBy

            //برای فیچر تایتل سورت داینامیک نداریم
            if (model.SearchFeatureConstantValues.Sorting == SortingBrands.FeatureTitle)
            {
                if (model.SearchFeatureConstantValues.SortingOrder == SortingOrder.Asc)
                {
                    featureConstantValues = featureConstantValues.OrderBy(x => x.Feature.Title);
                }
                else
                {
                    featureConstantValues = featureConstantValues.OrderByDescending(x => x.Feature.Title);
                }
            }
            else
            {
                featureConstantValues = featureConstantValues.CreateOrderByExpression(model.SearchFeatureConstantValues.Sorting.ToString(),
                    model.SearchFeatureConstantValues.SortingOrder.ToString());
            }

            #endregion

            var paginationResult = await GenericPaginationAsync(featureConstantValues, model.Pagination);

            return new()
            {
                FeatureConstantValues = await _mapper.ProjectTo<ShowFeatureConstantValueViewModel>(
                    paginationResult.Query
                ).ToListAsync(),
                Pagination = paginationResult.Pagination
            };
        }

        public Task<bool> CheckNonConstantValue(long categoryId, List<long> featureIds)
        {
            return _featureConstantValues.Where(x => x.CategoryId == categoryId)
                .AnyAsync(x => featureIds.Contains(x.FeatureId));

        }

        public async Task<bool> CheckConstantValue(long categoryId, List<long> featureConstantValueIds)
        {
            var featureCount = await _featureConstantValues.Where(x => x.CategoryId == categoryId)
                //آیدی های غیرتکراری رو با گروپ بای حذف کن
                .GroupBy(x => x.FeatureId)
                .CountAsync(x => featureConstantValueIds.Contains(x.Key));

            return featureCount == featureConstantValueIds.Count;

        }

        public Task<List<FeatureConstantValueForCreateProductViewModel>> GetFeatureConstantValuesForCreateProduct(long categoryId)
        {
            return _mapper.ProjectTo<FeatureConstantValueForCreateProductViewModel>(_featureConstantValues
                .Where(x => x.CategoryId == categoryId)).ToListAsync();
        }

        public Task<List<ShowCategoryFeatureConstantValueViewModel>> GetFeatureConstantValuesByCategoryId(long categoryId)
        {
            return _mapper.ProjectTo<ShowCategoryFeatureConstantValueViewModel>(_featureConstantValues
                .Where(x => x.CategoryId == categoryId)).ToListAsync();
        }
    }
}
