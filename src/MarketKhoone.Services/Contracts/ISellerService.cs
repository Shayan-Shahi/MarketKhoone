using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Sellers;

namespace MarketKhoone.Services.Contracts
{
    public interface ISellerService : IGenericService<Seller>
    {
        Task<int> GetSellerCodeForCreateSeller();
        Task<ShowSellersViewModel> GetSellers(ShowSellersViewModel model);
        Task<SellerDetailsViewModel> GetSellerDetails(long sellerId);
        Task<Seller> GetSellerToRemoveInManagingSellers(long id);
        Task<List<string>> GetShopNamesForAutocomplete(string term);

        /// <summary>
        /// اگر کاربر داخل سیستم فروشنده باشد
        /// آیدی فروشنده برگشت داد میشه
        /// </summary>
        /// <returns></returns>
        Task<long?> GetSellerId2();

        Task<long> GetSellerId(long userId);
        Task<long> GetSellerId();
    }
}
