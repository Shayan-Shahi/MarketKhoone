using MarketKhoone.Entities;

namespace MarketKhoone.Services.Contracts
{
    public interface IUserListShortLinkService : IGenericService<UserListShortLink>
    {
        /// <summary>
        /// گرفتن لینک برای بخش افزودن لیست
        /// </summary>
        /// <returns></returns>
        Task<UserListShortLink> GetUserListShortLinkForCreateUserList();
    }
}
