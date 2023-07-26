using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Products;

namespace MarketKhoone.Services.Contracts
{
    public interface IParcelPostItemService : ICustomGenericService<ParcelPostItem>
    {
        Task<ShowProductsInProfileCommentViewModel> GetProductsInProfileComment(ShowProductsInProfileCommentViewModel model);


        Task<ShowProductsInProfileCommentViewModel> GetProductsInProfileComment(int pageNumber);
    }
}
