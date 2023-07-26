using AutoMapper;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class CommentScoreService : CustomGenericService<CommentScore>, ICommentScoreService
    {
        #region Constructor
        private DbSet<CommentScore> _commentScores;
        private readonly IMapper _mapper;
        public CommentScoreService(IUnitOfWork uow,
             IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _commentScores = uow.Set<CommentScore>();
        }
        #endregion

        public Task<List<LikedCommentByUserViewModel>> GetLikedCommentsLikedByUser(long userId, long[] commentIds)
        {
            return _mapper.ProjectTo<LikedCommentByUserViewModel>(_commentScores.Where(x => x.UserId == userId)
                .Where(x => commentIds.Contains(x.ProductCommentId))).ToListAsync();

        }
    }
}
