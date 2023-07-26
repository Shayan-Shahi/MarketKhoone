using MarketKhoone.Entities;
using MarketKhoone.ViewModels;
using MarketKhoone.ViewModels.ProductComments;
using MarketKhoone.ViewModels.Products;

namespace MarketKhoone.Services.Contracts
{
    public interface IProductCommentService : IGenericService<ProductComment>
    {
        Task<int> GetAllNewProductComments();

        Task<ShowProductCommentsInProfile> GetCommentsInProfileComment(ShowProductCommentsInProfile model);
        Task<ShowProductCommentsInProfile> GetCommentsInProfileComment(int pageNumber);
        /// <summary>
        /// گرفتن نظرات محثولات به صورت صفحه بندی شده
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortBy"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<ProductCommentForProductInfoViewModel>> GetCommentsByPagination(long productId, int pageNumber, CommentsSortingForProductInfo sortBy, SortingOrder orderBy);
    }
}

