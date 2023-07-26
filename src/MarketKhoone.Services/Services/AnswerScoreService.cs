using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class AnswerScoreService : CustomGenericService<ProductQuestionAnswerScore>, IAnswerScoreService
    {
        #region Constructor

        private readonly DbSet<ProductQuestionAnswerScore> _productQuestionAnswerScores;
        private readonly IMapper _mapper;
        public AnswerScoreService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _productQuestionAnswerScores = uow.Set<ProductQuestionAnswerScore>();
        }
        #endregion

        public Task<List<LikedAnswerByUserViewModel>> GetLikedAnswersLikedByUser(long userId, long[] answerIds)
        {
            return _mapper.ProjectTo<LikedAnswerByUserViewModel>(_productQuestionAnswerScores
                .Where(x => x.UserId == userId)
                .Where(x => answerIds.Contains(x.AnswerId))).ToListAsync();
        }
    }
}
