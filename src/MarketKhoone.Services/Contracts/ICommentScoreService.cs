using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace MarketKhoone.Services.Contracts
{
    public interface ICommentScoreService : ICustomGenericService<CommentScore>
    {
        /// <summary>
        /// از داخل این کامنت ها کدامیک توسط این کاربر لایک و دیسلایک شده است؟
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="commentIds"></param>
        /// <returns></returns>
        Task<List<LikedCommentByUserViewModel>> GetLikedCommentsLikedByUser(long userId, long[] commentIds);
    }
}
