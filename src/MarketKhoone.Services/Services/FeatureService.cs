using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Features;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class FeatureService : GenericService<Feature>, IFeatureService
    {
        #region Constructor
        private readonly DbSet<Feature> _features;
        private readonly IMapper _mapper;
        public FeatureService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _features = uow.Set<Feature>();
        }
        #endregion

        public async Task<ShowFeaturesViewModel> GetCategoryFeatures(ShowFeaturesViewModel model)
        {

            var features = _features.AsNoTracking().AsQueryable();


            #region Search

            features = ExpressionHelpers.CreateSearchExpressions(features, model.SearchFeatures);

            #endregion


            #region OrderBy

            features = features.CreateOrderByExpression(model.SearchFeatures.SortingFeatures.ToString(),
                model.SearchFeatures.Sorting.ToString());
            #endregion


            #region Pagination

            var paginationResult = await GenericPaginationAsync(features, model.Pagination);

            #endregion


            #region Without AutoMapper
            // I commented this, because I have used AutoMapper in the following lines. Just in case, you don't 
            // like AutMapper, you can Uncomment this code and try it.

            //return new()
            //{
            //    Features = await paginationResult.Query
            //        .Select(x => new ShowFeatureViewModel()
            //        {
            //            Title = x.Title
            //        })
            //        .ToListAsync(),
            //    Pagination = paginationResult.Pagination
            //};

            #endregion

            #region With AutoMapper

            return new()
            {
                Features = await _mapper.ProjectTo<ShowFeatureViewModel>(paginationResult.Query).ToListAsync(),
                Pagination = paginationResult.Pagination
            };
            #endregion



        }

        public async Task<List<string>> AutocompleteSearch(string term)
        {
            return await _features.AsNoTracking()
                .Where(x => term.Contains(x.Title.Trim()))
                .Take(20)
                .Select(x => x.Title)
                .ToListAsync();
        }

        public async Task<Feature> FindByTitleAsync(string searchedTitle)
        {
            return await _features.SingleOrDefaultAsync(x => x.Title == searchedTitle);
        }
    }
}
