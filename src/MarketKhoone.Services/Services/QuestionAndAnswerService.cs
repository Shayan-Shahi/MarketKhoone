using AutoMapper;
using MarketKhoone.Common.Helpers;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.Products;
using MarketKhoone.ViewModels.QuestionsAndAnswers;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class QuestionAndAnswerService : GenericService<ProductQuestionAndAnswer>, IQuestionAndAnswerService
    {
        #region Constructor

        private readonly DbSet<ProductQuestionAndAnswer> _productQuestionAndAnswers;
        private readonly IMapper _mapper;
        public QuestionAndAnswerService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _productQuestionAndAnswers = uow.Set<ProductQuestionAndAnswer>();
        }
        #endregion


        public async Task<List<ProductQuestionForProductInfoViewModel>> GetQuestionsByPagination(long productId, int pageNumber, QuestionsSortingForProductInfo sortBy,
            SortingOrder orderBy)
        {
            var query = _productQuestionAndAnswers
                .Where(x => x.ParentId == null)
                .Where(x => x.IsConfirmed)
                .Where(x => x.ProductId == productId);

            #region OrderBy

            if (sortBy == QuestionsSortingForProductInfo.MostUseful)
            {

            }
            else
            {
                query = query.CreateOrderByExpression(sortBy.ToString(), orderBy.ToString());
            }
            #endregion

            query = await GenericPaginationAsync(query, pageNumber, 2);

            return await _mapper.ProjectTo<ProductQuestionForProductInfoViewModel>(query)
                .ToListAsync();

        }

        public Task<bool> IsExistsAndAnswer(long answerId)
        {
            return _productQuestionAndAnswers.AsNoTracking().AsQueryable()
                .Where(x => x.Id == answerId)
                .AnyAsync(x => x.ParentId != null);
        }
    }
}
