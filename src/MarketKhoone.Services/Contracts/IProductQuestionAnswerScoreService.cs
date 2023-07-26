using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace MarketKhoone.Services.Contracts
{
    public interface IProductQuestionAnswerScoreService : ICustomGenericService<ProductQuestionAnswerScore>
    {
        Task<List<LikedAnswerByUserViewModel>> GetLikedAnswersByUser(long userId, long[] questionIds);
    }
}
