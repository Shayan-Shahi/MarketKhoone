using AutoMapper;
using MarketKhoone.Entities;
using MarketKhoone.ViewModels.UserLists;

namespace Marketkhoone.Web.Mappings
{
    public class UserListMappingProfile : Profile
    {
        public UserListMappingProfile()
        {
            #region Parameters

            long productId = 0;

            #endregion

            this.CreateMap<UserList, UserListItemForProductInfoViewModel>()
                .ForMember(dest => dest.IsChecked,
                    options =>
                        options.MapFrom(src => src.UserListsProducts.Any(ulp => ulp.ProductId == productId)));


            this.CreateMap<AddUserListViewModel, UserList>()
                .AddTransform<string>(str => str != null ? str.Trim() : null);
        }
    }
}
