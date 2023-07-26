using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Wallets;

namespace Marketkhoone.Web.Mappings
{
    public class WalletMappingProfile : Profile
    {
        public WalletMappingProfile()
        {
            this.CreateMap<AddValueToWalletViewModel, Wallet>();
        }
    }
}
