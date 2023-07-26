using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace MarketKhoone.Services.Contracts
{
    public interface IAnswerScoreService : ICustomGenericService<ProductQuestionAnswerScore>
    {
        /// <summary>
        /// از داخل این جواب ها کدامیک توسط ان کاربر لایک و یا دیسلایک شده است؟
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="answerIds"></param>
        /// <returns></returns>
        Task<List<LikedAnswerByUserViewModel>> GetLikedAnswersLikedByUser(long userId, long[] answerIds);
    }
}
