using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketKhoone.DataLayer.Context;
using MarketKhoone.Entities;
using MarketKhoone.Services.Contracts;
using MarketKhoone.ViewModels.UserLists;
using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Services.Services
{
    public class UserListService : GenericService<UserList>, IUserListService
    {
        #region Constructor

        private readonly DbSet<UserList> _userLists;
        private readonly IMapper _mapper;
        public UserListService(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _mapper = mapper;
            _userLists = uow.Set<UserList>();
        }
        #endregion

        public Task<List<UserListItemForProductInfoViewModel>> GetUserListInProductInfo(long productId, long userId)
        {
            return _userLists.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .ProjectTo<UserListItemForProductInfoViewModel>
                (
                    configuration: _mapper.ConfigurationProvider,
                    parameters: new { productId = productId }
                )
                .ToListAsync();
        }

        public Task<List<long>> GetAllUserListIds(long userId)
        {
            return _userLists.Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .ToListAsync();
        }

        public bool CheckUserListIdsForUpdate(List<long> userListIds, List<long> allUserListIds)
        {
            var userListIdsCountFromDatabase = allUserListIds.LongCount(userListIds.Contains);
            return userListIds.Count == userListIdsCountFromDatabase;
        }

        public async Task<bool> CheckForTitleDuplicate(long userId, string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return true;
            }
            return await _userLists.Where(x => x.UserId == userId)
                .AnyAsync(x => x.Title == title.Trim());
        }
    }
}
