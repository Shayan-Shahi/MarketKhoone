using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class WalletService : GenericService<Wallet>, IWalletService
    {
        #region Constructor

        private readonly DbSet<Wallet> _wallets;
        public WalletService(IUnitOfWork uow) : base(uow)
        {
            _wallets = uow.Set<Wallet>();
        }
        #endregion

        public Task<Wallet> FindByTrackingNumber(long trackingNumber, long userId)
        {
            return _wallets.Where(x => x.TrackingNumber == trackingNumber)
                .SingleOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
