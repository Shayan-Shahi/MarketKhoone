using MarketKhoone.Entities;

namespace MarketKhoone.Services.Contracts
{
    public interface IWalletService : IGenericService<Wallet>
    {
        /// <summary>
        /// گرفتن کیف پ.ل براسا کد پیگیری
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Wallet> FindByTrackingNumber(long trackingNumber, long userId);
    }
}
