using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class ProductQuestionAnswerScoreService : CustomGenericService<ProductQuestionAnswerScore>, IProductQuestionAnswerScoreService
    {
        #region Constructor
        private readonly DbSet<ProductQuestionAnswerScore> _productQuestionAndAnswers;
        private readonly IMapper _mapper;
        public ProductQuestionAnswerScoreService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _productQuestionAndAnswers = uow.Set<ProductQuestionAnswerScore>();
        }
        #endregion

        public Task<List<LikedAnswerByUserViewModel>> GetLikedAnswersByUser(long userId, long[] questionIds)
        {
            return _mapper.ProjectTo<LikedAnswerByUserViewModel>(_productQuestionAndAnswers
                .Where(x => x.UserId == userId)
                .Where(x => questionIds.Contains(x.AnswerId))).ToListAsync();
        }
    }
}
