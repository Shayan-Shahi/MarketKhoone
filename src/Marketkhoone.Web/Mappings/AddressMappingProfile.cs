using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.Addresses;
using MarketKhoone.ViewModels.Carts;

namespace Marketkhoone.Web.Mappings
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            this.CreateMap<Address, AddressInCheckoutPageViewModel>();
            this.CreateMap<Address, ShowAddressInProfileViewModel>();
        }
    }
}
