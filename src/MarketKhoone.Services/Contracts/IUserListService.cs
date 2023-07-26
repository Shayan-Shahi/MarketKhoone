using MarketKhoone.Entities;
using MarketKhoone.ViewModels.UserLists;

namespace MarketKhoone.Services.Contracts
{
    public interface IUserListService : IGenericService<UserList>
    {
        /// <summary>
        /// نمایش لیست های کاربر
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserListItemForProductInfoViewModel>> GetUserListInProductInfo(long productId, long userId);
        /// <summary>
        /// تمامی لیست های کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<long>> GetAllUserListIds(long userId);

        bool CheckUserListIdsForUpdate(List<long> userListIds, List<long> allUserListIds);

        Task<bool> CheckForTitleDuplicate(long userId, string title);
    }
}
