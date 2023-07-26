using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Addresses;
using MarketKhoone.ViewModels.Carts;

namespace MarketKhoone.Services.Contracts
{
    public interface IAddressService : IGenericService<Address>
    {
        Task<List<ShowAddressInProfileViewModel>> GetAllUserAddresses();
        Task<bool> RemoveUserAddress(long id);
        Task<AddressInCheckoutPageViewModel> GetAddressForCheckoutPage(long userId);
        /// <summary>
        /// گرفتن آدرس کاربر برای یخش ایحاد سفارش
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<(bool hasUserAddress, long AddressId)> GetAddressForCreateOrderAndyPay(long userId);

        Task<EditAddressInProfileViewModel> GetForEdit(long id);
    }
}
