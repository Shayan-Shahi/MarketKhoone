using MarketKhoone.Entities;
using MarketKhoone.ViewModels.UserHistories;

namespace MarketKhoone.Services.Contracts
{
    public interface IUserHistoryService : ICustomGenericService<UserHistory>
    {
        Task<List<ShowUserHistoryViewModel>> GetUserHistories();
    }
}
